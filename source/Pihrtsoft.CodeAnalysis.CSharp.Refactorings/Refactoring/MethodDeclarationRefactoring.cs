// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
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
                && methodDeclaration.ParameterList?.Parameters.Count == 0;
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
