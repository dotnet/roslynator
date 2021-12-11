// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SwapBinaryOperandsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            BinaryExpressionInfo info = SyntaxInfo.BinaryExpressionInfo(binaryExpression);

            if (!info.Success)
                return;

            if (CanRefactor())
            {
                context.RegisterRefactoring(
                    "Swap operands",
                    ct =>
                    {
                        BinaryExpressionSyntax newBinaryExpression = SyntaxRefactorings.SwapBinaryOperands(binaryExpression);

                        newBinaryExpression = newBinaryExpression.WithOperatorToken(newBinaryExpression.OperatorToken.WithNavigationAnnotation());

                        return context.Document.ReplaceNodeAsync(binaryExpression, newBinaryExpression, ct);
                    },
                    RefactoringDescriptors.SwapBinaryOperands);
            }

            bool CanRefactor()
            {
                SyntaxKind kind = binaryExpression.Kind();

                switch (kind)
                {
                    case SyntaxKind.LogicalAndExpression:
                    case SyntaxKind.LogicalOrExpression:
                    case SyntaxKind.BitwiseAndExpression:
                    case SyntaxKind.BitwiseOrExpression:
                    case SyntaxKind.ExclusiveOrExpression:
                    case SyntaxKind.AddExpression:
                    case SyntaxKind.MultiplyExpression:
                        {
                            return !info.Left.IsKind(kind);
                        }
                    case SyntaxKind.EqualsExpression:
                    case SyntaxKind.NotEqualsExpression:
                        {
                            return !info.Right.IsKind(
                                SyntaxKind.NullLiteralExpression,
                                SyntaxKind.TrueLiteralExpression,
                                SyntaxKind.FalseLiteralExpression);
                        }
                    case SyntaxKind.GreaterThanExpression:
                    case SyntaxKind.GreaterThanOrEqualExpression:
                    case SyntaxKind.LessThanExpression:
                    case SyntaxKind.LessThanOrEqualExpression:
                        {
                            return true;
                        }
                }

                return false;
            }
        }
    }
}
