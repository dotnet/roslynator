// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CodeAnalysis.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseIsKindInsteadOfKindComparisonAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, CodeAnalysisDiagnosticRules.UseIsKindInsteadOfKindComparison);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeBinaryExpression(f), SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
    }

    private static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        var binaryExpression = (BinaryExpressionSyntax)context.Node;

        if (binaryExpression.ContainsDiagnostics)
            return;

        ExpressionSyntax outermost = GetOutermostLogicalOrOrSelf(binaryExpression);

        if (outermost.IsKind(SyntaxKind.LogicalOrExpression))
        {
            var logicalOr = (BinaryExpressionSyntax)outermost;

            if (!TryGetKindComparisonChain(
                logicalOr,
                context.SemanticModel,
                context.CancellationToken,
                out ImmutableArray<IsKindExpressionInfo> infos))
            {
                AnalyzeSingleComparison(context, binaryExpression);
                return;
            }

            if (!binaryExpression.Equals(infos[0].IsKindExpression))
                return;

            DiagnosticHelpers.ReportDiagnostic(
                context,
                CodeAnalysisDiagnosticRules.UseIsKindInsteadOfKindComparison,
                logicalOr);
        }
        else
        {
            AnalyzeSingleComparison(context, binaryExpression);
        }
    }

    private static void AnalyzeSingleComparison(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax binaryExpression)
    {
        IsKindExpressionInfo info = IsKindExpressionInfo.Create(
            binaryExpression,
            context.SemanticModel,
            cancellationToken: context.CancellationToken);

        if (!info.Success)
            return;

        if (!IsKindComparisonStyle(info.Style))
            return;

        if (!IsConstantSyntaxKind(info.KindExpression, context.SemanticModel, context.CancellationToken))
            return;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            CodeAnalysisDiagnosticRules.UseIsKindInsteadOfKindComparison,
            info.IsKindExpression);
    }

    private static bool TryGetKindComparisonChain(
        BinaryExpressionSyntax logicalOr,
        SemanticModel semanticModel,
        CancellationToken cancellationToken,
        out ImmutableArray<IsKindExpressionInfo> infos)
    {
        infos = default;

        BinaryExpressionInfo binaryExpressionInfo = SyntaxInfo.BinaryExpressionInfo(logicalOr);

        if (!binaryExpressionInfo.Success)
            return false;

        ImmutableArray<IsKindExpressionInfo>.Builder builder = ImmutableArray.CreateBuilder<IsKindExpressionInfo>();
        ExpressionSyntax commonExpression = null;

        foreach (ExpressionSyntax expression in binaryExpressionInfo.AsChain())
        {
            IsKindExpressionInfo info = IsKindExpressionInfo.Create(
                expression,
                semanticModel,
                cancellationToken: cancellationToken);

            if (!info.Success)
                return false;

            if (info.Style is not (IsKindExpressionStyle.Kind or IsKindExpressionStyle.KindConditional))
                return false;

            if (!IsConstantSyntaxKind(info.KindExpression, semanticModel, cancellationToken))
                return false;

            if (commonExpression is null)
            {
                commonExpression = info.Expression;
            }
            else if (!CSharpFactory.AreEquivalent(commonExpression, info.Expression))
            {
                return false;
            }

            builder.Add(info);
        }

        if (builder.Count < 2)
            return false;

        infos = builder.ToImmutable();
        return true;
    }

    private static ExpressionSyntax GetOutermostLogicalOrOrSelf(ExpressionSyntax expression)
    {
        expression = expression.WalkUpParentheses();

        while (expression.Parent.IsKind(SyntaxKind.LogicalOrExpression))
            expression = ((ExpressionSyntax)expression.Parent).WalkUpParentheses();

        return expression;
    }

    private static bool IsKindComparisonStyle(IsKindExpressionStyle style)
    {
        return style is IsKindExpressionStyle.Kind
            or IsKindExpressionStyle.KindConditional
            or IsKindExpressionStyle.NotKind
            or IsKindExpressionStyle.NotKindConditional;
    }

    private static bool IsConstantSyntaxKind(
        ExpressionSyntax expression,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        Optional<object> optionalConstantValue = semanticModel.GetConstantValue(expression, cancellationToken);

        return optionalConstantValue.HasValue
            && optionalConstantValue.Value is ushort;
    }
}
