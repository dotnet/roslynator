// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class MethodDeclarationRefactoring
    {
        public static bool CanConvertToReadOnlyProperty(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration.ReturnType?.IsVoid() == false
                && methodDeclaration.ParameterList?.Parameters.Count == 0
                && methodDeclaration.TypeParameterList == null;
        }

        public static async Task<Document> ConvertToReadOnlyPropertyAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PropertyDeclarationSyntax propertyDeclaration = ConvertToReadOnlyProperty(methodDeclaration)
                .WithTriviaFrom(methodDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(methodDeclaration, propertyDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        public static PropertyDeclarationSyntax ConvertToReadOnlyProperty(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            if (methodDeclaration.ExpressionBody != null)
            {
                return PropertyDeclaration(
                    methodDeclaration.AttributeLists,
                    methodDeclaration.Modifiers,
                    methodDeclaration.ReturnType,
                    methodDeclaration.ExplicitInterfaceSpecifier,
                    methodDeclaration.Identifier,
                    null,
                    methodDeclaration.ExpressionBody,
                    null);
            }
            else
            {
                return PropertyDeclaration(
                    methodDeclaration.AttributeLists,
                    methodDeclaration.Modifiers,
                    methodDeclaration.ReturnType,
                    methodDeclaration.ExplicitInterfaceSpecifier,
                    methodDeclaration.Identifier,
                    CreateAccessorList(methodDeclaration));
            }
        }

        private static AccessorListSyntax CreateAccessorList(MethodDeclarationSyntax method)
        {
            if (method.Body != null)
            {
                bool singleline = method.Body.Statements.Count == 1
                    && method.Body.Statements[0].IsSingleline();

                return CreateAccessorList(Block(method.Body?.Statements), singleline)
                    .WithOpenBraceToken(method.Body.OpenBraceToken)
                    .WithCloseBraceToken(method.Body.CloseBraceToken);
            }

            return CreateAccessorList(Block(), singleline: true);
        }

        private static AccessorListSyntax CreateAccessorList(BlockSyntax block, bool singleline)
        {
            AccessorListSyntax accessorList =
                AccessorList(
                    SingletonList(
                        AccessorDeclaration(
                            SyntaxKind.GetAccessorDeclaration,
                            block)));

            if (singleline)
                accessorList = WhitespaceOrEndOfLineRemover.RemoveFrom(accessorList);

            return accessorList;
        }

        internal static void RenameAccordingToTypeName(
            MethodDeclarationSyntax methodDeclaration,
            CodeRefactoringContext context,
            SemanticModel semanticModel)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (methodDeclaration.ReturnType?.IsVoid() == false
                && methodDeclaration.Identifier.Span.Contains(context.Span))
            {
                string newName = NamingHelper.CreateIdentifierName(methodDeclaration.ReturnType, semanticModel);

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
    }
}
