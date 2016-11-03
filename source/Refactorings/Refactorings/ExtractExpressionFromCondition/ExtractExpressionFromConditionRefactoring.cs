// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExtractExpressionFromConditionRefactoring
    {
        public static ExpressionSyntax GetNewCondition(
            ExpressionSyntax condition,
            ExpressionSyntax expression)
        {
            var binaryExpression = (BinaryExpressionSyntax)expression.Parent;

            if (expression.Equals(binaryExpression.Left))
            {
                return binaryExpression.Right;
            }
            else
            {
                if (binaryExpression.Equals(condition))
                {
                    return binaryExpression.Left.TrimTrailingTrivia();
                }
                else
                {
                    return binaryExpression.Left;
                }
            }
        }

        public static BinaryExpressionSyntax GetCondition(BinaryExpressionSyntax binaryExpression, SyntaxKind statementKind)
        {
            SyntaxKind kind = binaryExpression.Kind();

            while (binaryExpression.Parent != null)
            {
                SyntaxKind parentKind = binaryExpression.Parent.Kind();

                if (parentKind == kind)
                {
                    binaryExpression = (BinaryExpressionSyntax)binaryExpression.Parent;
                    continue;
                }
                else if (parentKind == statementKind)
                {
                    return binaryExpression;
                }

                return null;
            }

            return null;
        }
    }
}
