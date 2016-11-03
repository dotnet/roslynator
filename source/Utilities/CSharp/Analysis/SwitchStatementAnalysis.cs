// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    public static class SwitchStatementAnalysis
    {
        public static SwitchStatementAnalysisResult Analyze(SwitchStatementSyntax switchStatement)
        {
            if (switchStatement == null)
                throw new ArgumentNullException(nameof(switchStatement));

            return new SwitchStatementAnalysisResult(switchStatement);
        }

        public static SwitchSectionAnalysisResult AnalyzeSection(SwitchSectionSyntax section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            SyntaxList<StatementSyntax> statements = section.Statements;

            if (statements.Count > 1)
            {
                return  SwitchSectionAnalysisResult.AddBraces;
            }
            else if (statements.Count == 1)
            {
                if (statements[0].IsKind(SyntaxKind.Block))
                {
                    return SwitchSectionAnalysisResult.RemoveBraces;
                }
                else
                {
                    return SwitchSectionAnalysisResult.AddBraces;
                }
            }

            return SwitchSectionAnalysisResult.None;
        }

        public static bool CanAddBraces(SwitchSectionSyntax section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            SyntaxList<StatementSyntax> statements = section.Statements;

            if (statements.Count > 1)
            {
                return true;
            }
            else if (statements.Count == 1 && !statements[0].IsKind(SyntaxKind.Block))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CanRemoveBraces(SwitchSectionSyntax section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            SyntaxList<StatementSyntax> statements = section.Statements;

            return statements.Count == 1
                && statements[0].IsKind(SyntaxKind.Block);
        }
    }
}
