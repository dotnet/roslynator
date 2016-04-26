// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Analysis
{
    public static class SwitchStatementAnalysis
    {
        public static SwitchStatementAnalysisResult Analyze(SwitchStatementSyntax switchStatement)
        {
            if (switchStatement == null)
                throw new ArgumentNullException(nameof(switchStatement));

            return new SwitchStatementAnalysisResult(switchStatement);
        }

        public static bool CanAddBracesToSection(SwitchSectionSyntax section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            if (section.Statements.Count > 1)
            {
                return true;
            }
            else if (section.Statements.Count == 1 && !section.Statements[0].IsKind(SyntaxKind.Block))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CanRemoveBracesFromSection(SwitchSectionSyntax section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            return section.Statements.Count == 1
                && section.Statements[0].IsKind(SyntaxKind.Block);
        }
    }
}
