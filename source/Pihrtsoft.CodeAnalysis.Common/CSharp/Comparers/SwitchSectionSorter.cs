// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Comparers
{
    public sealed class SwitchSectionSorter : IComparer<SwitchSectionSyntax>
    {
        public int Compare(SwitchSectionSyntax x, SwitchSectionSyntax y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (x.Labels.Count == 0)
            {
                if (y.Labels.Count == 0)
                    return 0;
                else
                    return -1;
            }

            if (y.Labels.Count == 0)
                return 1;

            if (x.Labels.Any(SyntaxKind.DefaultSwitchLabel))
                return 1;

            if (y.Labels.Any(SyntaxKind.DefaultSwitchLabel))
                return -1;

            int result = x.Labels.Count.CompareTo(y.Labels.Count);

            if (result != 0)
                return result;

            return string.Compare(
                ((CaseSwitchLabelSyntax)x.Labels[0]).Value?.ToString(),
                ((CaseSwitchLabelSyntax)y.Labels[0]).Value?.ToString(),
                StringComparison.CurrentCulture);
        }
    }
}
