// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.ReplacePropertyWithMethod
{
    internal static class ReplacePropertyWithMethodRefactoring
    {
        private static readonly string[] _prefixes = new string[]
        {
            "Is",
            "Has",
            "Are",
            "Can",
            "Allow",
            "Supports",
            "Should",
            "Get",
            "Set"
        };

        public static void ComputeRefactoring(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (CanRefactor(context, propertyDeclaration))
            {
                context.RegisterRefactoring(
                    $"Replace '{propertyDeclaration.Identifier.ValueText}' with method",
                    cancellationToken => RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }
        }

        public static bool CanRefactor(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            AccessorDeclarationSyntax accessor = propertyDeclaration.AccessorList?.Accessors.SingleOrDefault(shouldThrow: false);

            if (accessor?.Kind() != SyntaxKind.GetAccessorDeclaration)
                return false;

            if (accessor.BodyOrExpressionBody() != null)
                return true;

            return context.SupportsCSharp6
                && accessor.IsAutoImplemented()
                && propertyDeclaration.Initializer?.Value != null;
        }

        public static async Task<Solution> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax property,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Solution solution = document.Solution();

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(property, cancellationToken);

            string methodName = GetMethodName(property);

            ImmutableArray<DocumentReferenceInfo> infos = await SyntaxFinder.FindReferencesByDocumentAsync(propertySymbol, solution, allowCandidate: false, cancellationToken: cancellationToken).ConfigureAwait(false);

            foreach (DocumentReferenceInfo info in infos)
            {
                var rewriter = new ReplacePropertyWithMethodSyntaxRewriter(info.References, methodName, property);

                SyntaxNode newRoot = rewriter.Visit(info.Root);

                solution = solution.WithDocumentSyntaxRoot(info.Document.Id, newRoot);
            }

            if (!infos.Any(f => f.Document.Id == document.Id))
            {
                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                SyntaxNode newRoot = root.ReplaceNode(property, ToMethodDeclaration(property));

                solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);
            }

            return solution;
        }

        public static MethodDeclarationSyntax ToMethodDeclaration(PropertyDeclarationSyntax property)
        {
            AccessorDeclarationSyntax getter = property.Getter();

            BlockSyntax getterBody = getter.Body;

            BlockSyntax methodBody = null;

            if (getterBody != null)
            {
                methodBody = Block(getterBody.Statements);
            }
            else
            {
                ArrowExpressionClauseSyntax getterExpressionBody = getter.ExpressionBody;

                if (getterExpressionBody != null)
                {
                    methodBody = Block(ReturnStatement(getterExpressionBody.Expression));
                }
                else
                {
                    methodBody = Block(ReturnStatement(property.Initializer.Value));
                }
            }

            methodBody = methodBody.WithTrailingTrivia(property.GetTrailingTrivia());

            MethodDeclarationSyntax method = MethodDeclaration(
                property.AttributeLists,
                property.Modifiers,
                property.Type,
                property.ExplicitInterfaceSpecifier,
                Identifier(GetMethodName(property)).WithLeadingTrivia(property.Identifier.LeadingTrivia),
                default(TypeParameterListSyntax),
                ParameterList().WithTrailingTrivia(property.Identifier.TrailingTrivia),
                default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                methodBody,
                default(ArrowExpressionClauseSyntax));

            return method
                .WithLeadingTrivia(property.GetLeadingTrivia())
                .WithFormatterAnnotation();
        }

        public static string GetMethodName(PropertyDeclarationSyntax propertyDeclaration)
        {
            string methodName = propertyDeclaration.Identifier.ValueText;

            if (!_prefixes.Any(prefix => StringUtility.HasPrefix(methodName, prefix)))
                methodName = "Get" + methodName;

            return methodName;
        }
    }
}
