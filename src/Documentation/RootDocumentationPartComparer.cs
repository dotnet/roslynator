// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace Roslynator.Documentation
{
    internal sealed class RootDocumentationPartComparer : IComparer<RootDocumentationParts>
    {
        private RootDocumentationPartComparer()
        {
        }

        public static RootDocumentationPartComparer Instance { get; } = new RootDocumentationPartComparer();

        public int Compare(RootDocumentationParts x, RootDocumentationParts y)
        {
            return GetRank(x).CompareTo(GetRank(y));
        }

        private static int GetRank(RootDocumentationParts part)
        {
            switch (part)
            {
                case RootDocumentationParts.Content:
                    return 1;
                case RootDocumentationParts.Namespaces:
                    return 2;
                case RootDocumentationParts.Classes:
                    return 3;
                case RootDocumentationParts.StaticClasses:
                    return 4;
                case RootDocumentationParts.Structs:
                    return 5;
                case RootDocumentationParts.Interfaces:
                    return 6;
                case RootDocumentationParts.Enums:
                    return 7;
                case RootDocumentationParts.Delegates:
                    return 8;
                case RootDocumentationParts.Other:
                    return 9;
            }

            Debug.Fail(part.ToString());

            return 0;
        }
    }
}
