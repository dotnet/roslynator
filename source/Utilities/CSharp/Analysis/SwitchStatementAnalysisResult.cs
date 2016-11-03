// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    public class SwitchStatementAnalysisResult
    {
        public SwitchStatementAnalysisResult(SwitchStatementSyntax switchStatement)
        {
            if (switchStatement == null)
                throw new ArgumentNullException(nameof(switchStatement));

            foreach (SwitchSectionSyntax section in switchStatement.Sections)
            {
                if (CanAddBraces && CanRemoveBraces)
                    break;

                if (!CanAddBraces)
                    CanAddBraces = SwitchStatementAnalysis.CanAddBraces(section);

                if (!CanRemoveBraces)
                    CanRemoveBraces = SwitchStatementAnalysis.CanRemoveBraces(section);
            }
        }

        public bool CanAddBraces { get; }
        public bool CanRemoveBraces { get; }
    }
}
