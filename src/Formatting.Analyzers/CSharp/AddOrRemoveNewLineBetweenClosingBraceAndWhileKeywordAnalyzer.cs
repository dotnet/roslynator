// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AddOrRemoveNewLineBetweenClosingBraceAndWhileKeywordAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AddNewLineBetweenClosingBraceAndWhileKeyword,
                    DiagnosticDescriptors.RemoveNewLineBetweenClosingBraceAndWhileKeyword);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeDoStatement, SyntaxKind.DoStatement);
        }

        private static void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            var doStatement = (DoStatementSyntax)context.Node;

            StatementSyntax statement = doStatement.Statement;

            if (!statement.IsKind(SyntaxKind.Block))
                return;

            SyntaxTriviaList trailingTrivia = statement.GetTrailingTrivia();

            if (trailingTrivia.Count == 0
                || trailingTrivia.SingleOrDefault(shouldThrow: false).IsWhitespaceTrivia())
            {
                if (!doStatement.WhileKeyword.LeadingTrivia.Any()
                    && !context.IsAnalyzerSuppressed(DiagnosticDescriptors.AddNewLineBetweenClosingBraceAndWhileKeyword))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AddNewLineBetweenClosingBraceAndWhileKeyword,
                        Location.Create(doStatement.SyntaxTree, new TextSpan(statement.FullSpan.End, 0)));
                }
            }
            else if (SyntaxTriviaAnalysis.IsOptionalWhitespaceThenEndOfLineTrivia(trailingTrivia))
            {
                if (doStatement.WhileKeyword.LeadingTrivia.IsEmptyOrWhitespace()
                    && !context.IsAnalyzerSuppressed(DiagnosticDescriptors.RemoveNewLineBetweenClosingBraceAndWhileKeyword))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveNewLineBetweenClosingBraceAndWhileKeyword,
                        Location.Create(doStatement.SyntaxTree, new TextSpan(trailingTrivia.Last().SpanStart, 0)));
                }
            }
        }
    }
}
