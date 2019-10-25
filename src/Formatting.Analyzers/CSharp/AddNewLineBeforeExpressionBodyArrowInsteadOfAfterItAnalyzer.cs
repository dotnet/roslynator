// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AddNewLineBeforeExpressionBodyArrowInsteadOfAfterItAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddNewLineBeforeExpressionBodyArrowInsteadOfAfterIt); }
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

            if (!SyntaxTriviaAnalysis.IsEmptyOrSingleWhitespaceTrivia(previousToken.TrailingTrivia))
                return;

            if (arrowToken.LeadingTrivia.Any())
                return;

            if (!SyntaxTriviaAnalysis.IsOptionalWhitespaceThenEndOfLineTrivia(arrowToken.TrailingTrivia))
                return;

            if (!SyntaxTriviaAnalysis.IsEmptyOrSingleWhitespaceTrivia(arrowExpressionClause.Expression.GetLeadingTrivia()))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddNewLineBeforeExpressionBodyArrowInsteadOfAfterIt, arrowToken);
        }
    }
}
