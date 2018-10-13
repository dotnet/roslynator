// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal static class TypeKindExtensions
    {
        public static NamespaceDocumentationParts ToNamespaceDocumentationPart(this TypeKind typeKind)
        {
            switch (typeKind)
            {
                case TypeKind.Class:
                    return NamespaceDocumentationParts.Classes;
                case TypeKind.Delegate:
                    return NamespaceDocumentationParts.Delegates;
                case TypeKind.Enum:
                    return NamespaceDocumentationParts.Enums;
                case TypeKind.Interface:
                    return NamespaceDocumentationParts.Interfaces;
                case TypeKind.Struct:
                    return NamespaceDocumentationParts.Structs;
            }

            throw new InvalidOperationException($"Unknown enum value '{typeKind.ToString()}'.");
        }
    }
}
