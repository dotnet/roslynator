// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.SyntaxInfo;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CombineEnumerableWhereAndAnyRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            CancellationToken cancellationToken)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SimpleMemberInvocationExpressionInfo(invocationExpression);
            SimpleMemberInvocationExpressionInfo invocationInfo2 = SimpleMemberInvocationExpressionInfo(invocationInfo.Expression);

            SingleParameterLambdaExpressionInfo lambda = SingleParameterLambdaExpressionInfo((LambdaExpressionSyntax)invocationInfo.Arguments.First().Expression);
            SingleParameterLambdaExpressionInfo lambda2 = SingleParameterLambdaExpressionInfo((LambdaExpressionSyntax)invocationInfo2.Arguments.First().Expression);

            BinaryExpressionSyntax logicalAnd = CSharpFactory.LogicalAndExpression(
                ((ExpressionSyntax)lambda2.Body).Parenthesize(),
                ((ExpressionSyntax)lambda.Body).Parenthesize());

            InvocationExpressionSyntax newNode = invocationInfo2.InvocationExpression
                .ReplaceNode(invocationInfo2.Name, invocationInfo.Name.WithTriviaFrom(invocationInfo2.Name))
                .WithArgumentList(invocationInfo2.ArgumentList.ReplaceNode((ExpressionSyntax)lambda2.Body, logicalAnd));

            return document.ReplaceNodeAsync(invocationExpression, newNode, cancellationToken);
        }
    }
}
