// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    public enum TypeDocumentationParts
    {
        None = 0,
        Content = 1,
        ContainingNamespace = 2,
        ContainingAssembly = 4,
        ObsoleteMessage = 8,
        Summary = 16,
        Declaration = 32,
        TypeParameters = 64,
        Parameters = 128,
        ReturnValue = 256,
        Inheritance = 512,
        Attributes = 1024,
        Derived = 2048,
        Implements = 4096,
        Examples = 8192,
        Remarks = 16384,
        Constructors = 32768,
        Fields = 65536,
        Indexers = 131072,
        Properties = 262144,
        Methods = 524288,
        Operators = 1048576,
        Events = 2097152,
        ExplicitInterfaceImplementations = 4194304,
        ExtensionMethods = 8388608,
        Classes = 16777216,
        Structs = 33554432,
        Interfaces = 67108864,
        Enums = 134217728,
        Delegates = 268435456,
        NestedTypes = Classes | Structs | Interfaces | Enums | Delegates,
        SeeAlso = 536870912,
        AllExceptNestedTypes = All & ~NestedTypes,
        All = Content | ContainingNamespace | ContainingAssembly | ObsoleteMessage | Summary | Declaration | TypeParameters | Parameters | ReturnValue | Inheritance | Attributes | Derived | Implements | Examples | Remarks | Constructors | Fields | Indexers | Properties | Methods | Operators | Events | ExplicitInterfaceImplementations | ExtensionMethods | Classes | Structs | Interfaces | Enums | Delegates | SeeAlso
    }
}
