// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BooleanLiteralDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveRedundantBooleanLiteral,
                    DiagnosticDescriptors.SimplifyBooleanComparison,
                    DiagnosticDescriptors.SimplifyBooleanComparisonFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeEqualsExpression, SyntaxKind.EqualsExpression);
            context.RegisterSyntaxNodeAction(AnalyzeNotEqualsExpression, SyntaxKind.NotEqualsExpression);
            context.RegisterSyntaxNodeAction(AnalyzeLogicalAndExpression, SyntaxKind.LogicalAndExpression);
            context.RegisterSyntaxNodeAction(AnalyzeLogicalOrExpression, SyntaxKind.LogicalOrExpression);
        }

        private void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax left = binaryExpression.Left;

            if (left?.IsMissing == false)
            {
                ExpressionSyntax right = binaryExpression.Right;

                if (right?.IsMissing == false)
                    AnalyzeEqualsNotEquals(context, binaryExpression, left, right, SyntaxKind.FalseLiteralExpression, SyntaxKind.TrueLiteralExpression);
            }
        }

        private void AnalyzeNotEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax left = binaryExpression.Left;

            if (left?.IsMissing == false)
            {
                ExpressionSyntax right = binaryExpression.Right;

                if (right?.IsMissing == false)
                    AnalyzeEqualsNotEquals(context, binaryExpression, left, right, SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression);
            }
        }

        private void AnalyzeLogicalAndExpression(SyntaxNodeAnalysisContext context)
        {
            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax left = binaryExpression.Left;

            if (left?.IsMissing == false)
            {
                ExpressionSyntax right = binaryExpression.Right;

                if (right?.IsMissing == false)
                    AnalyzeLogicalAndLogicalOr(context, binaryExpression, left, right, SyntaxKind.TrueLiteralExpression);
            }
        }

        private void AnalyzeLogicalOrExpression(SyntaxNodeAnalysisContext context)
        {
            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax left = binaryExpression.Left;

            if (left?.IsMissing == false)
            {
                ExpressionSyntax right = binaryExpression.Right;

                if (right?.IsMissing == false)
                    AnalyzeLogicalAndLogicalOr(context, binaryExpression, left, right, SyntaxKind.FalseLiteralExpression);
            }
        }

        private static void AnalyzeEqualsNotEquals(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax left,
            ExpressionSyntax right,
            SyntaxKind kind,
            SyntaxKind kind2)
        {
            SyntaxKind leftKind = left.Kind();

            if (leftKind == kind)
            {
                switch (AnalyzeExpression(right, context.SemanticModel, context.CancellationToken))
                {
                    case AnalysisResult.Boolean:
                        {
                            SimplifyBooleanComparisonRefactoring.ReportDiagnostic(context, binaryExpression, left, right, fadeOut: true);
                            break;
                        }
                    case AnalysisResult.LogicalNotWithNullableBoolean:
                        {
                            SimplifyBooleanComparisonRefactoring.ReportDiagnostic(context, binaryExpression, left, right, fadeOut: false);
                            break;
                        }
                }
            }
            else if (leftKind == kind2)
            {
                switch (AnalyzeExpression(right, context.SemanticModel, context.CancellationToken))
                {
                    case AnalysisResult.Boolean:
                        {
                            RemoveRedundantBooleanLiteralRefactoring.ReportDiagnostic(context, binaryExpression, left, right);
                            break;
                        }
                    case AnalysisResult.LogicalNotWithNullableBoolean:
                        {
                            SimplifyBooleanComparisonRefactoring.ReportDiagnostic(context, binaryExpression, left, right, fadeOut: false);
                            break;
                        }
                }
            }
            else
            {
                SyntaxKind rightKind = right.Kind();

                if (rightKind == kind)
                {
                    switch (AnalyzeExpression(left, context.SemanticModel, context.CancellationToken))
                    {
                        case AnalysisResult.Boolean:
                            {
                                SimplifyBooleanComparisonRefactoring.ReportDiagnostic(context, binaryExpression, left, right, fadeOut: true);
                                break;
                            }
                        case AnalysisResult.LogicalNotWithNullableBoolean:
                            {
                                SimplifyBooleanComparisonRefactoring.ReportDiagnostic(context, binaryExpression, left, right, fadeOut: false);
                                break;
                            }
                    }
                }
                else if (rightKind == kind2)
                {
                    switch (AnalyzeExpression(left, context.SemanticModel, context.CancellationToken))
                    {
                        case AnalysisResult.Boolean:
                            {
                                RemoveRedundantBooleanLiteralRefactoring.ReportDiagnostic(context, binaryExpression, left, right);
                                break;
                            }
                        case AnalysisResult.LogicalNotWithNullableBoolean:
                            {
                                SimplifyBooleanComparisonRefactoring.ReportDiagnostic(context, binaryExpression, left, right, fadeOut: false);
                                break;
                            }
                    }
                }
            }
        }

        private static void AnalyzeLogicalAndLogicalOr(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax left,
            ExpressionSyntax right,
            SyntaxKind kind)
        {
            if (left.IsKind(kind)
                || right.IsKind(kind))
            {
                RemoveRedundantBooleanLiteralRefactoring.ReportDiagnostic(context, binaryExpression, left, right);
            }
        }

        private static AnalysisResult AnalyzeExpression(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    return AnalysisResult.BooleanLiteral;
                case SyntaxKind.LogicalNotExpression:
                    {
                        var logicalNot = (PrefixUnaryExpressionSyntax)expression;

                        ExpressionSyntax operand = logicalNot.Operand;

                        if (operand != null)
                        {
                            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(operand, cancellationToken).ConvertedType;

                            if (typeSymbol != null)
                            {
                                if (typeSymbol.IsBoolean())
                                {
                                    return AnalysisResult.Boolean;
                                }
                                else if (typeSymbol.IsNullableOf(SpecialType.System_Boolean))
                                {
                                    return AnalysisResult.LogicalNotWithNullableBoolean;
                                }
                            }
                        }

                        break;
                    }
                default:
                    {
                        if (semanticModel
                            .GetTypeInfo(expression, cancellationToken)
                            .ConvertedType?
                            .IsBoolean() == true)
                        {
                            return AnalysisResult.Boolean;
                        }

                        break;
                    }
            }

            return AnalysisResult.None;
        }

        private enum AnalysisResult
        {
            None,
            BooleanLiteral,
            Boolean,
            LogicalNotWithNullableBoolean
        }
    }
}
