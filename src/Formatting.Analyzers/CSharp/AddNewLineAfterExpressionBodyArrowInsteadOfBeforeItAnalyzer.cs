// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AddNewLineAfterExpressionBodyArrowInsteadOfBeforeItAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddNewLineAfterExpressionBodyArrowInsteadOfBeforeIt); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeArrowExpressionClause, SyntaxKind.ArrowExpressionClause);
        }

        private static void AnalyzeArrowExpressionClause(SyntaxNodeAnalysisContext context)
        {
            var arrowExpressionClause = (ArrowExpressionClauseSyntax)context.Node;

            SyntaxToken arrowToken = arrowExpressionClause.ArrowToken;

            SyntaxToken previousToken = arrowToken.GetPreviousToken();

            if (!SyntaxTriviaAnalysis.IsOptionalWhitespaceThenEndOfLineTrivia(previousToken.TrailingTrivia))
                return;

            if (!SyntaxTriviaAnalysis.IsEmptyOrSingleWhitespaceTrivia(arrowToken.LeadingTrivia))
                return;

            if (!SyntaxTriviaAnalysis.IsEmptyOrSingleWhitespaceTrivia(arrowToken.TrailingTrivia))
                return;

            if (arrowExpressionClause.Expression.GetLeadingTrivia().Any())
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddNewLineAfterExpressionBodyArrowInsteadOfBeforeIt, arrowToken);
        }
    }
}
