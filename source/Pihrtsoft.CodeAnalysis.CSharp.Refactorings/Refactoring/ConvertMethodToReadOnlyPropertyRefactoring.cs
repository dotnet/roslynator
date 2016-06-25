// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ConvertMethodToReadOnlyPropertyRefactoring
    {
        public static bool CanRefactor(MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration.ReturnType?.IsVoid() == false
                && methodDeclaration.ParameterList?.Parameters.Count == 0
                && methodDeclaration.TypeParameterList == null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PropertyDeclarationSyntax propertyDeclaration = Refactor(methodDeclaration)
                .WithTriviaFrom(methodDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(methodDeclaration, propertyDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static PropertyDeclarationSyntax Refactor(MethodDeclarationSyntax methodDeclaration)
        {
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
    }
}
