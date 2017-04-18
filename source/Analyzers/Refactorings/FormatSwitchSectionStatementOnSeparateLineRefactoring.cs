// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatSwitchSectionStatementOnSeparateLineRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, SwitchSectionSyntax switchSection)
        {
            SyntaxList<StatementSyntax> statements = switchSection.Statements;

            if (statements.Any())
            {
                SyntaxList<SwitchLabelSyntax> labels = switchSection.Labels;

                if (labels.Any())
                {
                    StatementSyntax statement = statements.First();

                    if (switchSection.SyntaxTree.IsSingleLineSpan(TextSpan.FromBounds(labels.Last().Span.End, statement.SpanStart)))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.FormatSwitchSectionStatementOnSeparateLine,
                            statement);
                    }
                }
            }
        }
    }
}
