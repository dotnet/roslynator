// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analyzers
{
    internal static class FormatEachStatementOnSeparateLineAnalyzer
    {
        public static void AnalyzeStatements(SyntaxNodeAnalysisContext context, SyntaxList<StatementSyntax> statements)
        {
            if (statements.Count == 0)
                return;

            if (statements.Count == 1 && !statements[0].IsKind(SyntaxKind.Block))
                return;

            int previousIndex = statements[0].GetSpanEndLine();

            for (int i = 1; i < statements.Count; i++)
            {
                if (!statements[i].IsKind(SyntaxKind.Block)
                    && !statements[i].IsKind(SyntaxKind.EmptyStatement)
                    && statements[i].GetSpanStartLine() == previousIndex)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.FormatEachStatementOnSeparateLine,
                        statements[i].GetLocation());
                }

                if (statements[i].IsKind(SyntaxKind.Block))
                    AnalyzeStatements(context, ((BlockSyntax)statements[i]).Statements);

                previousIndex = statements[i].GetSpanEndLine();
            }
        }
    }
}
