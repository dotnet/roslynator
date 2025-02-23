﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BooleanLiteralAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(
                    ref _supportedDiagnostics,
                    DiagnosticRules.RemoveRedundantBooleanLiteral,
                    DiagnosticRules.SimplifyBooleanComparison,
                    DiagnosticRules.SimplifyBooleanComparisonFadeOut);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(
            c =>
            {
                if (DiagnosticHelpers.IsAnyEffective(c, DiagnosticRules.RemoveRedundantBooleanLiteral, DiagnosticRules.SimplifyBooleanComparison))
                    AnalyzeEqualsExpression(c);
            },
            SyntaxKind.EqualsExpression);

        context.RegisterSyntaxNodeAction(
            c =>
            {
                if (DiagnosticHelpers.IsAnyEffective(c, DiagnosticRules.RemoveRedundantBooleanLiteral, DiagnosticRules.SimplifyBooleanComparison))
                    AnalyzeNotEqualsExpression(c);
            },
            SyntaxKind.NotEqualsExpression);

        context.RegisterSyntaxNodeAction(
            c =>
            {
                if (DiagnosticHelpers.IsAnyEffective(c, DiagnosticRules.RemoveRedundantBooleanLiteral, DiagnosticRules.SimplifyBooleanComparison))
                    AnalyzeLogicalAndExpression(c);
            },
            SyntaxKind.LogicalAndExpression);

        context.RegisterSyntaxNodeAction(
            c =>
            {
                if (DiagnosticHelpers.IsAnyEffective(c, DiagnosticRules.RemoveRedundantBooleanLiteral, DiagnosticRules.SimplifyBooleanComparison))
                    AnalyzeLogicalOrExpression(c);
            },
            SyntaxKind.LogicalOrExpression);

        context.RegisterSyntaxNodeAction(
            c =>
            {
                if (DiagnosticRules.RemoveRedundantBooleanLiteral.IsEffective(c))
                    AnalyzeForStatement(c);
            },
            SyntaxKind.ForStatement);

        context.RegisterSyntaxNodeAction(
            c =>
            {
                if (DiagnosticRules.SimplifyBooleanComparison.IsEffective(c))
                    AnalyzeIsPatternExpression(c);
            },
            SyntaxKind.IsPatternExpression);
    }

    private static void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
    {
        var forStatement = (ForStatementSyntax)context.Node;

        ExpressionSyntax condition = forStatement.Condition;

        if (condition?.Kind() == SyntaxKind.TrueLiteralExpression)
            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.RemoveRedundantBooleanLiteral, condition, condition.ToString());
    }

    private static void AnalyzeIsPatternExpression(SyntaxNodeAnalysisContext context)
    {
        var isPattern = (IsPatternExpressionSyntax)context.Node;

        PatternSyntax pattern = isPattern.Pattern;

        if (isPattern.Pattern.IsKind(SyntaxKind.NotPattern))
        {
            var notPattern = (UnaryPatternSyntax)isPattern.Pattern;

            pattern = notPattern.Pattern;
        }

        if (pattern is ConstantPatternSyntax constantPattern)
        {
            ExpressionSyntax expression = constantPattern.Expression;

            if (expression.IsKind(SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression)
                && context.SemanticModel.GetTypeSymbol(
                    isPattern.Expression,
                    context.CancellationToken)?
                    .SpecialType == SpecialType.System_Boolean)
            {
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.SimplifyBooleanComparison, isPattern);
            }
        }
    }

    private static void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
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

    private static void AnalyzeNotEqualsExpression(SyntaxNodeAnalysisContext context)
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

    private static void AnalyzeLogicalAndExpression(SyntaxNodeAnalysisContext context)
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

    private static void AnalyzeLogicalOrExpression(SyntaxNodeAnalysisContext context)
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
                    SimplifyBooleanComparisonAnalysis.ReportDiagnostic(context, binaryExpression, left, right, fadeOut: true);
                    break;
                }
                case AnalysisResult.LogicalNotWithNullableBoolean:
                {
                    SimplifyBooleanComparisonAnalysis.ReportDiagnostic(context, binaryExpression, left, right, fadeOut: false);
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
                    RemoveRedundantBooleanLiteralAnalysis.ReportDiagnostic(context, binaryExpression, left, right);
                    break;
                }
                case AnalysisResult.LogicalNotWithNullableBoolean:
                {
                    SimplifyBooleanComparisonAnalysis.ReportDiagnostic(context, binaryExpression, left, right, fadeOut: false);
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
                        SimplifyBooleanComparisonAnalysis.ReportDiagnostic(context, binaryExpression, left, right, fadeOut: true);
                        break;
                    }
                    case AnalysisResult.LogicalNotWithNullableBoolean:
                    {
                        SimplifyBooleanComparisonAnalysis.ReportDiagnostic(context, binaryExpression, left, right, fadeOut: false);
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
                        RemoveRedundantBooleanLiteralAnalysis.ReportDiagnostic(context, binaryExpression, left, right);
                        break;
                    }
                    case AnalysisResult.LogicalNotWithNullableBoolean:
                    {
                        SimplifyBooleanComparisonAnalysis.ReportDiagnostic(context, binaryExpression, left, right, fadeOut: false);
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
            RemoveRedundantBooleanLiteralAnalysis.ReportDiagnostic(context, binaryExpression, left, right);
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

                if (operand is not null)
                {
                    ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(operand, cancellationToken).ConvertedType;

                    if (typeSymbol is not null)
                    {
                        if (typeSymbol.SpecialType == SpecialType.System_Boolean)
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
                    .SpecialType == SpecialType.System_Boolean)
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
        None = 0,
        BooleanLiteral = 1,
        Boolean = 2,
        LogicalNotWithNullableBoolean = 3,
    }
}
