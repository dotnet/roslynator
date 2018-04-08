// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseRegularStringLiteralInsteadOfVerbatimStringLiteralRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            int start = node.SpanStart;

            if (node.IsKind(SyntaxKind.InterpolatedStringExpression))
                start++;

            ExpressionSyntax newNode = SyntaxFactory.ParseExpression(node.ToFullString().Remove(start - node.FullSpan.Start, 1));

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }
    }
}
