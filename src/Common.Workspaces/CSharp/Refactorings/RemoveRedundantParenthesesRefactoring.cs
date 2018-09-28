// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantParenthesesRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ParenthesizedExpressionSyntax parenthesizedExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax expression = parenthesizedExpression.Expression;

            SyntaxTriviaList leading = parenthesizedExpression.GetLeadingTrivia()
                .Concat(parenthesizedExpression.OpenParenToken.TrailingTrivia)
                .Concat(expression.GetLeadingTrivia())
                .ToSyntaxTriviaList();

            SyntaxTriviaList trailing = expression.GetTrailingTrivia()
                .Concat(parenthesizedExpression.CloseParenToken.LeadingTrivia)
                .Concat(parenthesizedExpression.GetTrailingTrivia())
                .ToSyntaxTriviaList();

            ExpressionSyntax newExpression = expression
                .WithLeadingTrivia(leading)
                .WithTrailingTrivia(trailing)
                .WithFormatterAnnotation();

            if (!leading.Any())
            {
                SyntaxNode parent = parenthesizedExpression.Parent;

                switch (parent.Kind())
                {
                    case SyntaxKind.ReturnStatement:
                        {
                            var returnStatement = (ReturnStatementSyntax)parent;

                            SyntaxToken returnKeyword = returnStatement.ReturnKeyword;

                            if (!returnKeyword.TrailingTrivia.Any())
                            {
                                ReturnStatementSyntax newNode = returnStatement.Update(returnKeyword.WithTrailingTrivia(Space), newExpression, returnStatement.SemicolonToken);

                                return document.ReplaceNodeAsync(returnStatement, newNode, cancellationToken);
                            }

                            break;
                        }
                    case SyntaxKind.YieldReturnStatement:
                        {
                            var yieldReturn = (YieldStatementSyntax)parent;

                            SyntaxToken returnKeyword = yieldReturn.ReturnOrBreakKeyword;

                            if (!returnKeyword.TrailingTrivia.Any())
                            {
                                YieldStatementSyntax newNode = yieldReturn.Update(yieldReturn.YieldKeyword, returnKeyword.WithTrailingTrivia(Space), newExpression, yieldReturn.SemicolonToken);

                                return document.ReplaceNodeAsync(yieldReturn, newNode, cancellationToken);
                            }

                            break;
                        }
                    case SyntaxKind.AwaitExpression:
                        {
                            var awaitExpression = (AwaitExpressionSyntax)parent;

                            SyntaxToken awaitKeyword = awaitExpression.AwaitKeyword;

                            if (!awaitKeyword.TrailingTrivia.Any())
                            {
                                AwaitExpressionSyntax newNode = awaitExpression.Update(awaitKeyword.WithTrailingTrivia(Space), newExpression);

                                return document.ReplaceNodeAsync(awaitExpression, newNode, cancellationToken);
                            }

                            break;
                        }
                }
            }

            return document.ReplaceNodeAsync(parenthesizedExpression, newExpression, cancellationToken);
        }
    }
}
