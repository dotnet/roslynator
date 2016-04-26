// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class SwitchSectionSyntaxExtensions
    {
        public static bool ContainsSingleBlock(this SwitchSectionSyntax switchSection)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            if (switchSection.Statements.Count != 1)
                return false;

            return switchSection.Statements[0].IsKind(SyntaxKind.Block);
        }

        public static bool ContainsSingleStatement(this SwitchSectionSyntax switchSection)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            if (switchSection.Statements.Count != 1)
                return false;

            return !switchSection.Statements[0].IsKind(SyntaxKind.Block);
        }
    }
}
