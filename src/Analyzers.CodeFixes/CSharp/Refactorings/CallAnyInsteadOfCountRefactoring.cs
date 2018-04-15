// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

            switch (binaryExpression.Kind())
            {
                case SyntaxKind.EqualsExpression:
                    {
                        // Count() == 0 >>> !Any()
                        newNode = ChangeInvokedMethodName(invocationExpression, "Any");
                        newNode = LogicalNotExpression(newNode.Parenthesize());
                        break;
                    }
                case SyntaxKind.NotEqualsExpression:
                    {
                        // Count() != 0 >>> Any()
                        newNode = ChangeInvokedMethodName(invocationExpression, "Any");
                        break;
                    }
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                    {
                        if (invocationExpression == left)
                        {
                            // Count() < 1 >>> !Any()
                            // Count() <= 0 >>> !Any()
                            newNode = ChangeInvokedMethodName(invocationExpression, "Any");
                            newNode = LogicalNotExpression(newNode.Parenthesize());
                        }
                        else
                        {
                            // 0 < Count() >>> Any()
                            // 1 <= Count() >>> Any()
                            newNode = ChangeInvokedMethodName(invocationExpression, "Any");
                        }

                        break;
                    }
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                    {
                        if (invocationExpression == left)
                        {
                            // Count() > 0 >>> Any()
                            // Count() >= 1 >>> Any()
                            newNode = ChangeInvokedMethodName(invocationExpression, "Any");
                        }
                        else
                        {
                            // 1 > Count() >>> !Any()
                            // 0 >= Count() >>> !Any()
                            newNode = ChangeInvokedMethodName(invocationExpression, "Any");
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
    }
}
