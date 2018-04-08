// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddArgumentListToObjectCreationRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ObjectCreationExpressionSyntax objectCreationExpression,
            CancellationToken cancellationToken)
        {
            ObjectCreationExpressionSyntax newNode = objectCreationExpression
                .WithType(objectCreationExpression.Type.WithoutTrailingTrivia())
                .WithArgumentList(
                    SyntaxFactory.ArgumentList()
                        .WithTrailingTrivia(objectCreationExpression.Type.GetTrailingTrivia()));

            return document.ReplaceNodeAsync(objectCreationExpression, newNode, cancellationToken);
        }
    }
}
