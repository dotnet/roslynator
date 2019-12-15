// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveUnnecessaryBracesAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveUnnecessaryBraces,
                    DiagnosticDescriptors.RemoveUnnecessaryBracesFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (startContext.IsAnalyzerSuppressed(DiagnosticDescriptors.RemoveUnnecessaryBraces))
                    return;

                startContext.RegisterSyntaxNodeAction(AnalyzerSwitchSection, SyntaxKind.SwitchSection);
            });
        }

        public static void AnalyzerSwitchSection(SyntaxNodeAnalysisContext context)
        {
            var switchSection = (SwitchSectionSyntax)context.Node;

            if (!(switchSection.Statements.SingleOrDefault(shouldThrow: false) is BlockSyntax block))
                return;

            SyntaxList<StatementSyntax> statements = block.Statements;

            StatementSyntax firstStatement = statements.FirstOrDefault();

            if (firstStatement == null)
                return;

            SyntaxToken openBrace = block.OpenBraceToken;

            if (!AnalyzeTrivia(openBrace.LeadingTrivia))
                return;

            if (!AnalyzeTrivia(openBrace.TrailingTrivia))
                return;

            if (!AnalyzeTrivia(firstStatement.GetLeadingTrivia()))
                return;

            StatementSyntax lastStatement = statements.Last();

            if (!AnalyzeTrivia(lastStatement.GetTrailingTrivia()))
                return;

            SyntaxToken closeBrace = block.CloseBraceToken;

            if (!AnalyzeTrivia(closeBrace.LeadingTrivia))
                return;

            if (!AnalyzeTrivia(closeBrace.TrailingTrivia))
                return;

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.RemoveUnnecessaryBraces, openBrace);
            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.RemoveUnnecessaryBracesFadeOut, closeBrace);

            bool AnalyzeTrivia(SyntaxTriviaList trivia)
            {
                return trivia.All(f => f.IsKind(SyntaxKind.WhitespaceTrivia, SyntaxKind.EndOfLineTrivia, SyntaxKind.SingleLineCommentTrivia));
            }
        }
    }
}
