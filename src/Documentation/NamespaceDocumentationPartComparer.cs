// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace Roslynator.Documentation
{
    internal sealed class NamespaceDocumentationPartComparer : IComparer<NamespaceDocumentationParts>
    {
        private NamespaceDocumentationPartComparer()
        {
        }

        public static NamespaceDocumentationPartComparer Instance { get; } = new NamespaceDocumentationPartComparer();

        public int Compare(NamespaceDocumentationParts x, NamespaceDocumentationParts y)
        {
            return GetRank(x).CompareTo(GetRank(y));
        }

        private static int GetRank(NamespaceDocumentationParts part)
        {
            switch (part)
            {
                case NamespaceDocumentationParts.Content:
                    return 1;
                case NamespaceDocumentationParts.ContainingNamespace:
                    return 2;
                case NamespaceDocumentationParts.Summary:
                    return 3;
                case NamespaceDocumentationParts.Examples:
                    return 4;
                case NamespaceDocumentationParts.Remarks:
                    return 5;
                case NamespaceDocumentationParts.Classes:
                    return 6;
                case NamespaceDocumentationParts.Structs:
                    return 7;
                case NamespaceDocumentationParts.Interfaces:
                    return 8;
                case NamespaceDocumentationParts.Enums:
                    return 9;
                case NamespaceDocumentationParts.Delegates:
                    return 10;
                case NamespaceDocumentationParts.SeeAlso:
                    return 11;
            }

            Debug.Fail(part.ToString());

            return 0;
        }
    }
}
