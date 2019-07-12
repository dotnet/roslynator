// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallConfigureAwaitRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            AwaitExpressionSyntax awaitExpression,
            CancellationToken cancellationToken = default)
        {
            ExpressionSyntax expression = awaitExpression.Expression;

            InvocationExpressionSyntax invocationExpression = InvocationExpression(
                SimpleMemberAccessExpression(
                    expression.WithoutTrailingTrivia().Parenthesize(),
                    IdentifierName("ConfigureAwait")),
                ArgumentList(
                    Token(SyntaxKind.OpenParenToken),
                    SingletonSeparatedList(Argument(FalseLiteralExpression())),
                    Token(default, SyntaxKind.CloseParenToken, expression.GetTrailingTrivia())));

            return document.ReplaceNodeAsync(expression, invocationExpression, cancellationToken);
        }
    }
}
