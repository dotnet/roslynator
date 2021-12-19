// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace Roslynator.Documentation
{
    internal sealed class RootDocumentationPartComparer : IComparer<RootDocumentationParts>
    {
        private RootDocumentationPartComparer()
        {
        }

        public static RootDocumentationPartComparer Instance { get; } = new();

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
                case RootDocumentationParts.ClassHierarchy:
                    return 3;
                case RootDocumentationParts.Types:
                    return 4;
                case RootDocumentationParts.Other:
                    return 5;
            }

            Debug.Fail(part.ToString());

            return 0;
        }
    }
}
