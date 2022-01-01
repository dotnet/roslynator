// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class PlaceNewLineAfterOrBeforeArrowTokenAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.PlaceNewLineAfterOrBeforeArrowToken);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeArrowExpressionClause(f), SyntaxKind.ArrowExpressionClause);
        }

        private static void AnalyzeArrowExpressionClause(SyntaxNodeAnalysisContext context)
        {
            var arrowExpressionClause = (ArrowExpressionClauseSyntax)context.Node;

            SyntaxToken arrowToken = arrowExpressionClause.ArrowToken;

            NewLinePosition newLinePosition = context.GetArrowTokenNewLinePosition();

            if (newLinePosition == NewLinePosition.None)
                return;

            FormattingSuggestion suggestion = FormattingAnalysis.AnalyzeNewLineBeforeOrAfter(arrowToken, arrowExpressionClause.Expression, newLinePosition);

            if (suggestion == FormattingSuggestion.AddNewLineBefore)
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.PlaceNewLineAfterOrBeforeArrowToken,
                    arrowToken.GetLocation(),
                    "before");
            }
            else if (suggestion == FormattingSuggestion.AddNewLineAfter)
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.PlaceNewLineAfterOrBeforeArrowToken,
                    arrowToken.GetLocation(),
                    properties: DiagnosticProperties.AnalyzerOption_Invert,
                    "after");
            }
        }
    }
}
