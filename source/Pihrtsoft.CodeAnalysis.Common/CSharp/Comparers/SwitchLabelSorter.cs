// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Comparers
{
    public sealed class SwitchLabelSorter : IComparer<SwitchLabelSyntax>
    {
        public int Compare(SwitchLabelSyntax x, SwitchLabelSyntax y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (x.IsKind(SyntaxKind.DefaultSwitchLabel))
                return 1;

            if (y.IsKind(SyntaxKind.DefaultSwitchLabel))
                return -1;

            return string.Compare(
                ((CaseSwitchLabelSyntax)x).Value?.ToString(),
                ((CaseSwitchLabelSyntax)y).Value?.ToString(),
                StringComparison.CurrentCulture);
        }
    }
}
