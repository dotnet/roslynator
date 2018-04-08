// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyLinqMethodChainRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            var invocation2 = (InvocationExpressionSyntax)memberAccess.Expression;

            var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

            InvocationExpressionSyntax newNode = invocation2.WithExpression(
                memberAccess2.WithName(memberAccess.Name.WithTriviaFrom(memberAccess2.Name)));

            IEnumerable<SyntaxTrivia> trivia = invocation.DescendantTrivia(TextSpan.FromBounds(invocation2.Span.End, invocation.Span.End));

            if (trivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
            {
                newNode = newNode.WithTrailingTrivia(trivia.Concat(invocation.GetTrailingTrivia()));
            }
            else
            {
                newNode = newNode.WithTrailingTrivia(invocation.GetTrailingTrivia());
            }

            return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
        }
    }
}
