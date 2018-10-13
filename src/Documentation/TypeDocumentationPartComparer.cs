// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace Roslynator.Documentation
{
    internal sealed class TypeDocumentationPartComparer : IComparer<TypeDocumentationParts>
    {
        private TypeDocumentationPartComparer()
        {
        }

        public static TypeDocumentationPartComparer Instance { get; } = new TypeDocumentationPartComparer();

        public int Compare(TypeDocumentationParts x, TypeDocumentationParts y)
        {
            return GetRank(x).CompareTo(GetRank(y));
        }

        private static int GetRank(TypeDocumentationParts part)
        {
            switch (part)
            {
                case TypeDocumentationParts.Content:
                    return 1;
                case TypeDocumentationParts.ContainingNamespace:
                    return 2;
                case TypeDocumentationParts.ContainingAssembly:
                    return 3;
                case TypeDocumentationParts.ObsoleteMessage:
                    return 4;
                case TypeDocumentationParts.Summary:
                    return 5;
                case TypeDocumentationParts.Declaration:
                    return 6;
                case TypeDocumentationParts.TypeParameters:
                    return 7;
                case TypeDocumentationParts.Parameters:
                    return 8;
                case TypeDocumentationParts.ReturnValue:
                    return 9;
                case TypeDocumentationParts.Inheritance:
                    return 10;
                case TypeDocumentationParts.Attributes:
                    return 11;
                case TypeDocumentationParts.Derived:
                    return 12;
                case TypeDocumentationParts.Implements:
                    return 13;
                case TypeDocumentationParts.Examples:
                    return 14;
                case TypeDocumentationParts.Remarks:
                    return 15;
                case TypeDocumentationParts.Constructors:
                    return 16;
                case TypeDocumentationParts.Fields:
                    return 17;
                case TypeDocumentationParts.Indexers:
                    return 18;
                case TypeDocumentationParts.Properties:
                    return 19;
                case TypeDocumentationParts.Methods:
                    return 20;
                case TypeDocumentationParts.Operators:
                    return 21;
                case TypeDocumentationParts.Events:
                    return 22;
                case TypeDocumentationParts.ExplicitInterfaceImplementations:
                    return 23;
                case TypeDocumentationParts.ExtensionMethods:
                    return 24;
                case TypeDocumentationParts.Classes:
                    return 25;
                case TypeDocumentationParts.Structs:
                    return 26;
                case TypeDocumentationParts.Interfaces:
                    return 27;
                case TypeDocumentationParts.Enums:
                    return 28;
                case TypeDocumentationParts.Delegates:
                    return 29;
                case TypeDocumentationParts.SeeAlso:
                    return 30;
            }

            Debug.Fail(part.ToString());

            return 0;
        }
    }
}
