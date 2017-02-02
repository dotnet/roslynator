// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{

    public class EnumMemberDeclarationComparer : IComparer<EnumMemberDeclarationSyntax>
    {
        public int Compare(EnumMemberDeclarationSyntax x, EnumMemberDeclarationSyntax y)
        {
            return ComparePrivate(x, y);
        }

        private static int ComparePrivate(EnumMemberDeclarationSyntax x, EnumMemberDeclarationSyntax y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            return string.Compare(x.Identifier.ValueText, y.Identifier.ValueText, StringComparison.CurrentCulture);
        }

        public static bool IsListSorted(IList<EnumMemberDeclarationSyntax> members)
        {
            if (members == null)
                throw new ArgumentNullException(nameof(members));

            for (int i = 0; i < members.Count - 1; i++)
            {
                if (ComparePrivate(members[i], members[i + 1]) > 0)
                    return false;
            }

            return true;
        }
    }
}
