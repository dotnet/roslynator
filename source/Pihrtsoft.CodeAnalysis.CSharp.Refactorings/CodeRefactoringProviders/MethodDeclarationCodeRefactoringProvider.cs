// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(MethodDeclarationCodeRefactoringProvider))]
    public class MethodDeclarationCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            MethodDeclarationSyntax methodDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MethodDeclarationSyntax>();

            if (methodDeclaration == null)
                return;

            ConvertToReadOnlyProperty(context, methodDeclaration);

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

            if (semanticModel == null)
                return;

            RenameMethodAccordingToTypeName(context, semanticModel, methodDeclaration);
        }

        private static void ConvertToReadOnlyProperty(CodeRefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.ReturnType == null || methodDeclaration.ReturnType.IsVoid())
                return;

            if (methodDeclaration.ParameterList?.Parameters.Count != 0)
                return;

            if (methodDeclaration.Body == null)
                return;

            if (methodDeclaration.Body.Span.IntersectsWith(context.Span))
                return;

            context.RegisterRefactoring(
                "Convert to read-only property",
                cancellationToken => ConvertToPropertyAsync(context.Document, methodDeclaration, cancellationToken));
        }

        private static void RenameMethodAccordingToTypeName(
            CodeRefactoringContext context,
            SemanticModel semanticModel,
            MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.ReturnType?.IsVoid() == false
                && methodDeclaration.Identifier.Span.Contains(context.Span))
            {
                string newName = NamingHelper.CreateIdentifierName(methodDeclaration.ReturnType, semanticModel);

                Debug.Assert(!string.IsNullOrEmpty(newName), methodDeclaration.ReturnType.ToString());

                if (!string.IsNullOrEmpty(newName))
                {
                    newName = "Get" + newName;

                    if (!string.Equals(newName, methodDeclaration.Identifier.ToString(), StringComparison.Ordinal))
                    {
                        ISymbol symbol = semanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

                        context.RegisterRefactoring(
                            $"Rename method to '{newName}'",
                            cancellationToken => symbol.RenameAsync(newName, context.Document, cancellationToken));
                    }
                }
            }
        }

        private static async Task<Document> ConvertToPropertyAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PropertyDeclarationSyntax propertyDeclaration = ConvertMethodToReadOnlyProperty(methodDeclaration)
                .WithTriviaFrom(methodDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(methodDeclaration, propertyDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static PropertyDeclarationSyntax ConvertMethodToReadOnlyProperty(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return PropertyDeclaration(
                methodDeclaration.AttributeLists,
                methodDeclaration.Modifiers,
                methodDeclaration.ReturnType,
                methodDeclaration.ExplicitInterfaceSpecifier,
                methodDeclaration.Identifier,
                AccessorList(
                    SingletonList(
                        AccessorDeclaration(
                            SyntaxKind.GetAccessorDeclaration,
                            GetMethodBody(methodDeclaration)))));
        }

        private static BlockSyntax GetMethodBody(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.Body != null)
                return methodDeclaration.Body;

            if (methodDeclaration.ExpressionBody != null)
                return Block(ReturnStatement(methodDeclaration.ExpressionBody.Expression));

            return Block();
        }
    }
}