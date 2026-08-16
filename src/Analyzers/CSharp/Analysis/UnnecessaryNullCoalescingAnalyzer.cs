// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnnecessaryNullCoalescingAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UnnecessaryNullCoalescing);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeCoalesceExpression(f), SyntaxKind.CoalesceExpression);
        context.RegisterSyntaxNodeAction(f => AnalyzeCoalesceAssignmentExpression(f), SyntaxKind.CoalesceAssignmentExpression);
    }

    private static void AnalyzeCoalesceExpression(SyntaxNodeAnalysisContext context)
    {
        var coalesceExpression = (BinaryExpressionSyntax)context.Node;

        if (coalesceExpression.SpanContainsDirectives())
            return;

        ExpressionSyntax left = coalesceExpression.Left;
        ExpressionSyntax right = coalesceExpression.Right;

        if (left?.IsMissing != false || right?.IsMissing != false)
            return;

        ExpressionSyntax coalesced = coalesceExpression.WalkUpParentheses();

        if (coalesced.Parent is BinaryExpressionSyntax outerCoalesce
            && outerCoalesce.IsKind(SyntaxKind.CoalesceExpression)
            && outerCoalesce.Right == coalesced
            && HasNotNullReferenceFlow(outerCoalesce.Left, context.SemanticModel, context.CancellationToken))
        {
            return;
        }

        if (!IsUnnecessaryNullCoalescingLeft(left, context.SemanticModel, context.CancellationToken))
            return;

        ReportDiagnostic(context, coalesceExpression.SyntaxTree, coalesceExpression.OperatorToken, right);
    }

    private static void AnalyzeCoalesceAssignmentExpression(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;

        if (assignment.SpanContainsDirectives())
            return;

        ExpressionSyntax left = assignment.Left;
        ExpressionSyntax right = assignment.Right;

        if (left?.IsMissing != false || right?.IsMissing != false)
            return;

        if (!IsUnnecessaryNullCoalescingLeft(left, context.SemanticModel, context.CancellationToken))
            return;

        ReportDiagnostic(context, assignment.SyntaxTree, assignment.OperatorToken, right);
    }

    private static bool IsUnnecessaryNullCoalescingLeft(
        ExpressionSyntax left,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        ExpressionSyntax walkedLeft = left.WalkDownParentheses();

        if (IsHandledBySimplifyCoalesceExpression(walkedLeft, semanticModel, cancellationToken))
            return false;

        return HasNotNullReferenceFlow(left, semanticModel, cancellationToken);
    }

    private static bool HasNotNullReferenceFlow(
        ExpressionSyntax expression,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        TypeInfo typeInfo = semanticModel.GetTypeInfo(expression, cancellationToken);

        if (typeInfo.Nullability.FlowState != NullableFlowState.NotNull)
            return false;

        ITypeSymbol type = typeInfo.Type;

        return type?.IsErrorType() == false
            && type.IsReferenceType;
    }

    private static bool IsHandledBySimplifyCoalesceExpression(
        ExpressionSyntax left,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        switch (left.Kind())
        {
            case SyntaxKind.ObjectCreationExpression:
            case SyntaxKind.AnonymousObjectCreationExpression:
            case SyntaxKind.ArrayCreationExpression:
            case SyntaxKind.ImplicitArrayCreationExpression:
            case SyntaxKind.InterpolatedStringExpression:
            case SyntaxKind.ThisExpression:
            case SyntaxKind.StringLiteralExpression:
            case SyntaxKind.TypeOfExpression:
                return true;
        }

        Optional<object> optional = semanticModel.GetConstantValue(left, cancellationToken);

        return optional.HasValue
            && optional.Value is not null;
    }

    private static void ReportDiagnostic(
        SyntaxNodeAnalysisContext context,
        SyntaxTree syntaxTree,
        SyntaxToken operatorToken,
        ExpressionSyntax right)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UnnecessaryNullCoalescing,
            Location.Create(syntaxTree, TextSpan.FromBounds(operatorToken.SpanStart, right.Span.End)));
    }
}
