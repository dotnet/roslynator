// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CombineEnumerableWhereMethodChainRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            var invocation2 = (InvocationExpressionSyntax)memberAccess.Expression;

            ExpressionSyntax expression1 = GetCondition(invocation);
            ExpressionSyntax expression2 = GetCondition(invocation2);

            InvocationExpressionSyntax newInvocation = invocation2.ReplaceNode(
                expression2,
                LogicalAndExpression(
                    expression2.Parenthesize(),
                    expression1.Parenthesize()));

            var newMemberAccess = (MemberAccessExpressionSyntax)newInvocation.Expression;

            SyntaxTriviaList trailingTrivia = invocation.GetTrailingTrivia();

            IEnumerable<SyntaxTrivia> trivia = invocation.DescendantTrivia(TextSpan.FromBounds(invocation2.Span.End, memberAccess.Name.SpanStart));

            if (trivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                trailingTrivia = trailingTrivia.InsertRange(0, trivia);

            newInvocation = newInvocation
                .WithExpression(newMemberAccess)
                .WithTrailingTrivia(trailingTrivia)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken);
        }

        private static ExpressionSyntax GetCondition(InvocationExpressionSyntax invocation)
        {
            var lambda = (LambdaExpressionSyntax)invocation.ArgumentList.Arguments.First().Expression;

            return (ExpressionSyntax)lambda.Body;
        }
    }
}
