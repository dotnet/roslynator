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
public sealed class PlaceNewLineAfterOrBeforeNullConditionalOperatorAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, FormattingDiagnosticRules.PlaceNewLineAfterOrBeforeNullConditionalOperator);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeConditionalAccess(f), SyntaxKind.ConditionalAccessExpression);
    }

    private static void AnalyzeConditionalAccess(SyntaxNodeAnalysisContext context)
    {
        var conditionalAccess = (ConditionalAccessExpressionSyntax)context.Node;

        ExpressionSyntax left = conditionalAccess.Expression;

        if (left.IsMissing)
            return;

        ExpressionSyntax right = conditionalAccess.WhenNotNull;

        if (right.IsMissing)
            return;

        NewLinePosition newLinePosition = context.GetNullConditionalOperatorNewLinePosition();

        TriviaBlock block = TriviaBlock.FromSurrounding(conditionalAccess.OperatorToken, right, newLinePosition);

        if (block.Success)
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                FormattingDiagnosticRules.PlaceNewLineAfterOrBeforeNullConditionalOperator,
                block.GetLocation(),
                (block.First.IsToken) ? "before" : "after");
        }
    }
}
