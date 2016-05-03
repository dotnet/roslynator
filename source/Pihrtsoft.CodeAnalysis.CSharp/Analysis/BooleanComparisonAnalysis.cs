// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Analysis
{
    internal static class BooleanComparisonAnalysis
    {
        public static BooleanComparisonAnalysisResult Analyze(
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
                if (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
                {
                    return Analyze(
                        binaryExpression,
                        SyntaxKind.FalseLiteralExpression,
                        SyntaxKind.TrueLiteralExpression,
                        semanticModel,
                        cancellationToken);
                }
                else if (binaryExpression.IsKind(SyntaxKind.NotEqualsExpression))
                {
                    return Analyze(
                        binaryExpression,
                        SyntaxKind.TrueLiteralExpression,
                        SyntaxKind.FalseLiteralExpression,
                        semanticModel,
                        cancellationToken);
                }
            }

            return BooleanComparisonAnalysisResult.None;
        }

        private static BooleanComparisonAnalysisResult Analyze(
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
                    return BooleanComparisonAnalysisResult.SimplifyBooleanComparison;
            }
            else if (left.IsKind(booleanLiteralKind2))
            {
                if (IsBooleanExpressionButNotBooleanLiteral(right, semanticModel, cancellationToken))
                    return BooleanComparisonAnalysisResult.RemoveRedundantBooleanComparison;
            }
            else if (right.IsKind(booleanLiteralKind))
            {
                if (IsBooleanExpressionButNotBooleanLiteral(left, semanticModel, cancellationToken))
                    return BooleanComparisonAnalysisResult.SimplifyBooleanComparison;
            }
            else if (right.IsKind(booleanLiteralKind2))
            {
                if (IsBooleanExpressionButNotBooleanLiteral(left, semanticModel, cancellationToken))
                    return BooleanComparisonAnalysisResult.RemoveRedundantBooleanComparison;
            }

            return BooleanComparisonAnalysisResult.None;
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

                        return typeSymbol?.SpecialType == SpecialType.System_Boolean;
                    }
            }
        }
    }
}
