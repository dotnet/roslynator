// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    public enum TypeDocumentationParts
    {
        None = 0,
        Content = 1,
        ContainingNamespace = 1 << 1,
        ContainingAssembly = 1 << 2,
        ObsoleteMessage = 1 << 3,
        Summary = 1 << 4,
        Declaration = 1 << 5,
        TypeParameters = 1 << 6,
        Parameters = 1 << 7,
        ReturnValue = 1 << 8,
        Inheritance = 1 << 9,
        Attributes = 1 << 10,
        Derived = 1 << 11,
        Implements = 1 << 12,
        Examples = 1 << 13,
        Remarks = 1 << 14,
        Constructors = 1 << 15,
        Fields = 1 << 16,
        Indexers = 1 << 17,
        Properties = 1 << 18,
        Methods = 1 << 19,
        Operators = 1 << 20,
        Events = 1 << 21,
        ExplicitInterfaceImplementations = 1 << 22,
        ExtensionMethods = 1 << 23,
        Classes = 1 << 24,
        Structs = 1 << 25,
        Interfaces = 1 << 26,
        Enums = 1 << 27,
        Delegates = 1 << 28,
        NestedTypes = Classes | Structs | Interfaces | Enums | Delegates,
        SeeAlso = 1 << 29,
        AppliesTo = 1 << 30,
        AllExceptNestedTypes = All & ~NestedTypes,
        All = int.MaxValue
    }
}
