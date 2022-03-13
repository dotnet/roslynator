// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [Flags]
    internal enum SymbolDisplayTypeDeclarationOptions
    {
        None = 0,
        IncludeModifiers = 1,
        IncludeAccessibility = 1 << 1,
        BaseType = 1 << 2,
        Interfaces = 1 << 3,
        BaseList = BaseType | Interfaces
    }
}
