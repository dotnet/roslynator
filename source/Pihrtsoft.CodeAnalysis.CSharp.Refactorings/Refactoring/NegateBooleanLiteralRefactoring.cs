// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class NegateBooleanLiteralRefactoring
    {
        public static async Task<Document> RefactoringAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            LiteralExpressionSyntax newNode = GetNewNode(literalExpression)
                .WithTriviaFrom(literalExpression);

            SyntaxNode newRoot = oldRoot.ReplaceNode(literalExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static LiteralExpressionSyntax GetNewNode(LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression.IsKind(SyntaxKind.TrueLiteralExpression))
                return SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
            else
                return SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression);
        }
    }
}
