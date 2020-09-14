// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AddNewLineBeforeExpressionBodyArrowInsteadOfAfterItOrViceVersaAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddNewLineBeforeExpressionBodyArrowInsteadOfAfterItOrViceVersa); }
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

            SyntaxToken previousToken = arrowToken.GetPreviousToken();

            if (SyntaxTriviaAnalysis.IsEmptyOrSingleWhitespaceTrivia(previousToken.TrailingTrivia))
            {
                if (!arrowToken.LeadingTrivia.Any()
                    && SyntaxTriviaAnalysis.IsOptionalWhitespaceThenEndOfLineTrivia(arrowToken.TrailingTrivia)
                    && SyntaxTriviaAnalysis.IsEmptyOrSingleWhitespaceTrivia(arrowExpressionClause.Expression.GetLeadingTrivia())
                    && context.IsAnalyzerSuppressed(AnalyzerOptions.AddNewLineAfterExpressionBodyArrowInsteadOfBeforeIt))
                {
                    DiagnosticHelpers.ReportDiagnostic(
                        context,
                        DiagnosticDescriptors.AddNewLineBeforeExpressionBodyArrowInsteadOfAfterItOrViceVersa,
                        arrowToken.GetLocation());
                }
            }
            else if (SyntaxTriviaAnalysis.IsOptionalWhitespaceThenEndOfLineTrivia(previousToken.TrailingTrivia))
            {
                if (SyntaxTriviaAnalysis.IsEmptyOrSingleWhitespaceTrivia(arrowToken.LeadingTrivia)
                    && SyntaxTriviaAnalysis.IsEmptyOrSingleWhitespaceTrivia(arrowToken.TrailingTrivia)
                    && !arrowExpressionClause.Expression.GetLeadingTrivia().Any()
                    && !context.IsAnalyzerSuppressed(AnalyzerOptions.AddNewLineAfterExpressionBodyArrowInsteadOfBeforeIt))
                {
                    DiagnosticHelpers.ReportDiagnostic(
                        context,
                        DiagnosticDescriptors.ReportOnly.AddNewLineAfterExpressionBodyArrowInsteadOfBeforeIt,
                        arrowToken.GetLocation(),
                        properties: DiagnosticProperties.AnalyzerOption_Invert);
                }
            }
        }
    }
}
