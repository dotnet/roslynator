// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeStringLiteralsRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            return binaryExpression.IsKind(SyntaxKind.AddExpression)
                && context.Span.IsBetweenSpans(binaryExpression)
                && StringLiteralChain.IsStringLiteralChain(binaryExpression);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            bool multiline = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var chain = new StringLiteralChain(binaryExpression);

            LiteralExpressionSyntax newNode = (multiline)
                ? chain.ToMultilineStringLiteral()
                : chain.ToStringLiteral();

            return await document.ReplaceNodeAsync(
                binaryExpression,
                newNode.WithFormatterAnnotation(),
                cancellationToken).ConfigureAwait(false);
        }
    }
}
