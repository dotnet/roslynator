// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.ReplaceMethodWithProperty
{
    internal static class ReplaceMethodWithPropertyRefactoring
    {
        public static bool CanRefactor(MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration.ReturnType?.IsVoid() == false
                && methodDeclaration.ParameterList?.Parameters.Count == 0
                && methodDeclaration.TypeParameterList == null
                && !methodDeclaration.Modifiers.ContainsAny(SyntaxKind.OverrideKeyword, SyntaxKind.AsyncKeyword);
        }

        public static async Task<Solution> RefactorAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Solution solution = document.Solution();

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

            string propertyName = methodDeclaration.Identifier.ValueText;

            ImmutableArray<DocumentReferenceInfo> infos = await SyntaxFinder.FindReferencesByDocumentAsync(methodSymbol, solution, allowCandidate: false, cancellationToken: cancellationToken).ConfigureAwait(false);

            foreach (DocumentReferenceInfo info in infos)
            {
                var rewriter = new ReplaceMethodWithPropertySyntaxRewriter(info.References, propertyName, methodDeclaration);

                SyntaxNode newRoot = rewriter.Visit(info.Root);

                solution = solution.WithDocumentSyntaxRoot(info.Document.Id, newRoot);
            }

            if (!infos.Any(f => f.Document.Id == document.Id))
            {
                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                SyntaxNode newRoot = root.ReplaceNode(methodDeclaration, ToPropertyDeclaration(methodDeclaration));

                solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);
            }

            return solution;
        }

        public static PropertyDeclarationSyntax ToPropertyDeclaration(MethodDeclarationSyntax methodDeclaration)
        {
            return ToPropertyDeclarationCore(methodDeclaration)
                .WithTriviaFrom(methodDeclaration)
                .WithFormatterAnnotation();
        }

        private static PropertyDeclarationSyntax ToPropertyDeclarationCore(MethodDeclarationSyntax methodDeclaration)
        {
            SyntaxToken identifier = methodDeclaration.Identifier.WithTriviaFrom(methodDeclaration.Identifier);

            ParameterListSyntax parameterList = methodDeclaration.ParameterList;

            if (parameterList?.IsMissing == false)
            {
                identifier = identifier.AppendToTrailingTrivia(
                    parameterList.OpenParenToken.GetAllTrivia().Concat(
                        parameterList.CloseParenToken.GetAllTrivia()));
            }

            if (methodDeclaration.ExpressionBody != null)
            {
                return PropertyDeclaration(
                    methodDeclaration.AttributeLists,
                    methodDeclaration.Modifiers,
                    methodDeclaration.ReturnType,
                    methodDeclaration.ExplicitInterfaceSpecifier,
                    identifier,
                    default(AccessorListSyntax),
                    methodDeclaration.ExpressionBody,
                    default(EqualsValueClauseSyntax),
                    methodDeclaration.SemicolonToken);
            }
            else
            {
                return PropertyDeclaration(
                    methodDeclaration.AttributeLists,
                    methodDeclaration.Modifiers,
                    methodDeclaration.ReturnType,
                    methodDeclaration.ExplicitInterfaceSpecifier,
                    identifier,
                    CreateAccessorList(methodDeclaration));
            }
        }

        private static AccessorListSyntax CreateAccessorList(MethodDeclarationSyntax method)
        {
            BlockSyntax body = method.Body;

            if (body != null)
            {
                SyntaxList<StatementSyntax> statements = body.Statements;

                bool singleline = statements.Count == 1
                    && body.DescendantTrivia(body.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia())
                    && statements[0].IsSingleLine();

                return CreateAccessorList(Block(body.Statements), singleline)
                    .WithOpenBraceToken(body.OpenBraceToken)
                    .WithCloseBraceToken(body.CloseBraceToken);
            }

            return CreateAccessorList(Block(), singleline: true);
        }

        private static AccessorListSyntax CreateAccessorList(BlockSyntax block, bool singleline)
        {
            AccessorListSyntax accessorList = AccessorList(GetAccessorDeclaration(block));

            if (singleline)
                accessorList = accessorList.RemoveWhitespace();

            return accessorList;
        }
    }
}
