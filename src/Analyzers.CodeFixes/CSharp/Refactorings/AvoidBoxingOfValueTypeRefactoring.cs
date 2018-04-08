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
    internal static class AvoidBoxingOfValueTypeRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax newNode = null;

            if (expression.Kind() == SyntaxKind.CharacterLiteralExpression)
            {
                var literalExpression = (LiteralExpressionSyntax)expression;

                newNode = StringLiteralExpression(literalExpression.Token.ValueText);
            }
            else
            {
                ParenthesizedExpressionSyntax newExpression = expression
                    .WithoutTrivia()
                    .Parenthesize();

                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                if (semanticModel.GetTypeSymbol(expression, cancellationToken).IsNullableType())
                {
                    newNode = ConditionalAccessExpression(
                        newExpression,
                        InvocationExpression(MemberBindingExpression(IdentifierName("ToString")), ArgumentList()));
                }
                else
                {
                    newNode = SimpleMemberInvocationExpression(
                        newExpression,
                        IdentifierName("ToString"),
                        ArgumentList());
                }
            }

            newNode = newNode.WithTriviaFrom(expression);

            return await document.ReplaceNodeAsync(expression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
