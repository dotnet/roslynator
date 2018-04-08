// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class AddBracesToSwitchSectionAnalysis
    {
        public const string Title = "Add braces to section";

        public static bool CanAddBraces(SwitchSectionSyntax section)
        {
            SyntaxList<StatementSyntax> statements = section.Statements;

            if (statements.Count > 1)
            {
                return true;
            }
            else if (statements.Count == 1 && statements[0].Kind() != SyntaxKind.Block)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
