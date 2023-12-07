// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PlaceNewLineAfterOrBeforeConditionalOperatorAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.PlaceNewLineAfterOrBeforeConditionalOperator);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeConditionalExpression(f), SyntaxKind.ConditionalExpression);
    }

    private static void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
    {
        var conditionalExpression = (ConditionalExpressionSyntax)context.Node;

        ExpressionSyntax condition = conditionalExpression.Condition;

        if (condition.IsMissing)
            return;

        ExpressionSyntax whenTrue = conditionalExpression.WhenTrue;

        if (whenTrue.IsMissing)
            return;

        NewLinePosition newLinePosition = context.GetConditionalExpressionNewLinePosition();

        TriviaBlock block = TriviaBlock.FromSurrounding(conditionalExpression.QuestionToken, whenTrue, newLinePosition);

        if (block.Success)
            ReportDiagnostic(context, block);

        block = TriviaBlock.FromSurrounding(conditionalExpression.ColonToken, conditionalExpression.WhenFalse, newLinePosition);

        if (block.Success)
            ReportDiagnostic(context, block);

        static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            TriviaBlock block)
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.PlaceNewLineAfterOrBeforeConditionalOperator,
                block.GetLocation(),
                (block.First.IsToken) ? "before" : "after");
        }
    }
}
