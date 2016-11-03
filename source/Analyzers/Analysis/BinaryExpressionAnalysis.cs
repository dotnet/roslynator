// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class BinaryExpressionAnalysis
    {
        public static BinaryExpressionAnalysisResult Analyze(
            BinaryExpressionSyntax binaryExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (binaryExpression == null)
                throw new ArgumentNullException(nameof(binaryExpression));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (binaryExpression.Left != null && binaryExpression.Right != null)
            {
                switch (binaryExpression.Kind())
                {
                    case SyntaxKind.EqualsExpression:
                        {
                            return AnalyzeComparison(
                                binaryExpression,
                                SyntaxKind.FalseLiteralExpression,
                                SyntaxKind.TrueLiteralExpression,
                                semanticModel,
                                cancellationToken);
                        }
                    case SyntaxKind.NotEqualsExpression:
                        {
                            return AnalyzeComparison(
                                binaryExpression,
                                SyntaxKind.TrueLiteralExpression,
                                SyntaxKind.FalseLiteralExpression,
                                semanticModel,
                                cancellationToken);
                        }
                    case SyntaxKind.LogicalAndExpression:
                        {
                            return AnalyzeLogicalExpression(binaryExpression, SyntaxKind.TrueLiteralExpression);
                        }
                    case SyntaxKind.LogicalOrExpression:
                        {
                            return AnalyzeLogicalExpression(binaryExpression, SyntaxKind.FalseLiteralExpression);
                        }
                }
            }

            return BinaryExpressionAnalysisResult.None;
        }

        private static BinaryExpressionAnalysisResult AnalyzeComparison(
            BinaryExpressionSyntax binaryExpression,
            SyntaxKind booleanLiteralKind,
            SyntaxKind booleanLiteralKind2,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            if (left.IsKind(booleanLiteralKind))
            {
                if (IsBooleanExpressionButNotBooleanLiteral(right, semanticModel, cancellationToken))
                    return BinaryExpressionAnalysisResult.SimplifyBooleanComparison;
            }
            else if (left.IsKind(booleanLiteralKind2))
            {
                if (IsBooleanExpressionButNotBooleanLiteral(right, semanticModel, cancellationToken))
                    return BinaryExpressionAnalysisResult.RemoveRedundantBooleanLiteral;
            }
            else if (right.IsKind(booleanLiteralKind))
            {
                if (IsBooleanExpressionButNotBooleanLiteral(left, semanticModel, cancellationToken))
                    return BinaryExpressionAnalysisResult.SimplifyBooleanComparison;
            }
            else if (right.IsKind(booleanLiteralKind2))
            {
                if (IsBooleanExpressionButNotBooleanLiteral(left, semanticModel, cancellationToken))
                    return BinaryExpressionAnalysisResult.RemoveRedundantBooleanLiteral;
            }

            return BinaryExpressionAnalysisResult.None;
        }

        private static BinaryExpressionAnalysisResult AnalyzeLogicalExpression(
            BinaryExpressionSyntax binaryExpression,
            SyntaxKind booleanLiteralKind)
        {
            if (binaryExpression.Left.IsKind(booleanLiteralKind)
                || binaryExpression.Right.IsKind(booleanLiteralKind))
            {
                return BinaryExpressionAnalysisResult.RemoveRedundantBooleanLiteral;
            }

            return BinaryExpressionAnalysisResult.None;
        }

        private static bool IsBooleanExpressionButNotBooleanLiteral(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    return false;
                case SyntaxKind.LogicalNotExpression:
                    return true;
                default:
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(expression, cancellationToken).ConvertedType;

                        return typeSymbol?.IsBoolean() == true;
                    }
            }
        }
    }
}
