// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    public enum IncludeContainingNamespaceFilter
    {
        None = 0,
        ClassHierarchy = 1,
        ContainingType = 1 << 1,
        Parameter = 1 << 2,
        ReturnType = 1 << 3,
        BaseType = 1 << 4,
        Attribute = 1 << 5,
        DerivedType = 1 << 6,
        ImplementedInterface = 1 << 7,
        ImplementedMember = 1 << 8,
        Exception = 1 << 9,
        SeeAlso = 1 << 10,
        All = int.MaxValue
    }
}
