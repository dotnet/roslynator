// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.RefactoringUtility;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallSkipAndAnyInsteadOfCountRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!(binaryExpression.Left is InvocationExpressionSyntax invocationExpression))
            {
                invocationExpression = (InvocationExpressionSyntax)binaryExpression.Right;
            }

            ExpressionSyntax left = binaryExpression.Left;

            ExpressionSyntax right = binaryExpression.Right;

            ExpressionSyntax newNode = null;

            switch (binaryExpression.Kind())
            {
                case SyntaxKind.LessThanExpression:
                    {
                        if (invocationExpression == left)
                        {
                            // Count() < i >>> !Skip(i - 1).Any()
                            newNode = CreateNewExpression(invocationExpression, SubtractExpression(right, NumericLiteralExpression(1)));
                            newNode = LogicalNotExpression(newNode.Parenthesize());
                        }
                        else
                        {
                            // i < Count() >>> Skip(i).Any()
                            newNode = CreateNewExpression(invocationExpression, left);
                        }

                        break;
                    }
                case SyntaxKind.LessThanOrEqualExpression:
                    {
                        if (invocationExpression == left)
                        {
                            // Count() <= i >>> !Skip(i).Any()
                            newNode = CreateNewExpression(invocationExpression, right);
                            newNode = LogicalNotExpression(newNode.Parenthesize());
                        }
                        else
                        {
                            // i <= Count() >>> Skip(i - 1).Any()
                            newNode = CreateNewExpression(invocationExpression, SubtractExpression(left, NumericLiteralExpression(1)));
                        }

                        break;
                    }
                case SyntaxKind.GreaterThanExpression:
                    {
                        if (invocationExpression == left)
                        {
                            // Count() > i >>> Skip(i).Any()
                            newNode = CreateNewExpression(invocationExpression, right);
                        }
                        else
                        {
                            // i > Count() >>> !Skip(i - 1).Any()
                            newNode = CreateNewExpression(invocationExpression, SubtractExpression(left, NumericLiteralExpression(1)));
                            newNode = LogicalNotExpression(newNode.Parenthesize());
                        }

                        break;
                    }
                case SyntaxKind.GreaterThanOrEqualExpression:
                    {
                        if (invocationExpression == left)
                        {
                            // Count() >= i >>> Skip(i - 1).Any()
                            newNode = CreateNewExpression(invocationExpression, SubtractExpression(right, NumericLiteralExpression(1)));
                        }
                        else
                        {
                            // i >= Count() >>> !Skip(i).Any()
                            newNode = CreateNewExpression(invocationExpression, left);
                            newNode = LogicalNotExpression(newNode.Parenthesize());
                        }

                        break;
                    }
            }

            newNode = newNode
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }

        private static ExpressionSyntax CreateNewExpression(
            InvocationExpressionSyntax invocationExpression,
            ExpressionSyntax expression)
        {
            InvocationExpressionSyntax newExpression = ChangeInvokedMethodName(invocationExpression, "Skip");

            newExpression = newExpression.AddArgumentListArguments(Argument(expression));

            return SimpleMemberInvocationExpression(newExpression, IdentifierName("Any"));
        }
    }
}
