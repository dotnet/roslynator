// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    public enum NamespaceDocumentationParts
    {
        None = 0,
        Content = 1,
        ContainingNamespace = 1 << 1,
        Summary = 1 << 2,
        Examples = 1 << 3,
        Remarks = 1 << 4,
        Classes = 1 << 5,
        Structs = 1 << 6,
        Interfaces = 1 << 7,
        Enums = 1 << 8,
        Delegates = 1 << 9,
        Namespaces = 1 << 10,
        SeeAlso = 1 << 11,
        All = int.MaxValue
    }
}
