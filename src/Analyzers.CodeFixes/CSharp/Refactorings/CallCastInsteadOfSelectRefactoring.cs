// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallCastInsteadOfSelectRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            CancellationToken cancellationToken)
        {
            var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

            ArgumentSyntax lastArgument = invocationExpression.ArgumentList.Arguments.Last();

            var lambdaExpression = (LambdaExpressionSyntax)lastArgument.Expression;

            GenericNameSyntax newName = GenericName(
                Identifier("Cast"),
                CallCastInsteadOfSelectAnalysis.GetCastExpression(lambdaExpression.Body).Type);

            InvocationExpressionSyntax newInvocationExpression = invocationExpression
                .RemoveNode(lastArgument)
                .WithExpression(memberAccessExpression.WithName(newName));

            return document.ReplaceNodeAsync(invocationExpression, newInvocationExpression, cancellationToken);
        }
    }
}
