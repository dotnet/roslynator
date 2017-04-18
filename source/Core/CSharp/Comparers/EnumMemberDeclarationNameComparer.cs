// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Comparers
{
    internal class EnumMemberDeclarationNameComparer : IComparer<EnumMemberDeclarationSyntax>
    {
        private EnumMemberDeclarationNameComparer()
        {
        }

        public static readonly EnumMemberDeclarationNameComparer Instance = new EnumMemberDeclarationNameComparer();

        public int Compare(EnumMemberDeclarationSyntax x, EnumMemberDeclarationSyntax y)
        {
            return CompareCore(x, y);
        }

        private static int CompareCore(EnumMemberDeclarationSyntax x, EnumMemberDeclarationSyntax y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            return string.Compare(x.Identifier.ValueText, y.Identifier.ValueText, StringComparison.CurrentCulture);
        }

        public static bool IsSorted(IEnumerable<EnumMemberDeclarationSyntax> members)
        {
            if (members == null)
                throw new ArgumentNullException(nameof(members));

            using (IEnumerator<EnumMemberDeclarationSyntax> en = members.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    EnumMemberDeclarationSyntax enumMember1 = en.Current;

                    while (en.MoveNext())
                    {
                        EnumMemberDeclarationSyntax enumMember2 = en.Current;

                        if (CompareCore(enumMember1, enumMember2) > 0)
                            return false;

                        enumMember1 = enumMember2;
                    }
                }
            }

            return true;
        }
    }
}
