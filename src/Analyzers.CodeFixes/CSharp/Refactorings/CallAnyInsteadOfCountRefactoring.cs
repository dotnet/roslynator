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
    internal static class CallAnyInsteadOfCountRefactoring
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

            SyntaxKind kind = binaryExpression.Kind();

            if (kind == SyntaxKind.EqualsExpression)
            {
                // Count() == 0 >>> !Any()
                newNode = ChangeInvokedMethodName(invocationExpression, "Any");
                newNode = LogicalNotExpression(newNode.Parenthesize());
            }
            else if (kind == SyntaxKind.NotEqualsExpression)
            {
                // Count() != 0 >>> Any()
                newNode = ChangeInvokedMethodName(invocationExpression, "Any");
            }
            else if (kind == SyntaxKind.LessThanExpression)
            {
                if (invocationExpression == left)
                {
                    // Count() < 1 >>> !Any()
                    // Count() < i >>> !Skip(i - 1).Any()
                    newNode = CreateNewExpression(invocationExpression, right, "1", subtract: true, negate: true);
                }
                else
                {
                    // 0 < Count() >>> Any()
                    // i < Count() >>> Skip(i).Any()
                    newNode = CreateNewExpression(invocationExpression, left, "0");
                }
            }
            else if (kind == SyntaxKind.LessThanOrEqualExpression)
            {
                if (invocationExpression == left)
                {
                    // Count() <= 0 >>> !Any()
                    // Count() <= i >>> !Skip(i).Any()
                    newNode = CreateNewExpression(invocationExpression, right, "0", negate: true);
                }
                else
                {
                    // 1 <= Count() >>> Any()
                    // i <= Count() >>> Skip(i - 1).Any()
                    newNode = CreateNewExpression(invocationExpression, left, "1", subtract: true);
                }
            }
            else if (kind == SyntaxKind.GreaterThanExpression)
            {
                if (invocationExpression == left)
                {
                    // Count() > 0 >>> Any()
                    // Count() > i >>> Skip(i).Any()
                    newNode = CreateNewExpression(invocationExpression, right, "0");
                }
                else
                {
                    // 1 > Count() >>> !Any()
                    // i > Count() >>> !Skip(i - 1).Any()
                    newNode = CreateNewExpression(invocationExpression, left, "1", subtract: true, negate: true);
                }
            }
            else if (kind == SyntaxKind.GreaterThanOrEqualExpression)
            {
                if (invocationExpression == left)
                {
                    // Count() >= 1 >>> Any()
                    // Count() >= i >>> Skip(i - 1).Any()
                    newNode = CreateNewExpression(invocationExpression, right, "1", subtract: true);
                }
                else
                {
                    // 0 >= Count() >>> !Any()
                    // i >= Count() >>> !Skip(i).Any()
                    newNode = CreateNewExpression(invocationExpression, left, "0", negate: true);
                }
            }

            newNode = newNode
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }

        private static ExpressionSyntax CreateNewExpression(
            InvocationExpressionSyntax invocationExpression,
            ExpressionSyntax expression,
            string numericValue,
            bool subtract = false,
            bool negate = false)
        {
            ExpressionSyntax newExpression;

            if (expression.IsNumericLiteralExpression(numericValue))
            {
                newExpression = ChangeInvokedMethodName(invocationExpression, "Any");
            }
            else
            {
                if (subtract)
                    expression = SubtractExpression(expression, NumericLiteralExpression(1));

                newExpression = ChangeInvokedMethodName(invocationExpression, "Skip")
                    .AddArgumentListArguments(Argument(expression));

                newExpression = SimpleMemberInvocationExpression(newExpression, IdentifierName("Any"));
            }

            if (negate)
                newExpression = LogicalNotExpression(newExpression.Parenthesize());

            return newExpression;
        }
    }
}
