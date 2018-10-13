// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    public enum OmitContainingNamespaceParts
    {
        None = 0,
        Root = 1,
        ContainingType = 2,
        ReturnType = 4,
        BaseType = 8,
        Attribute = 16,
        DerivedType = 32,
        ImplementedInterface = 64,
        ImplementedMember = 128,
        Exception = 256,
        SeeAlso = 512,
        All = Root | ContainingType | ReturnType | BaseType | Attribute | DerivedType | ImplementedInterface | ImplementedMember | Exception | SeeAlso
    }
}
