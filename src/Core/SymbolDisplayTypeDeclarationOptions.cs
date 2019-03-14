// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [Flags]
    internal enum SymbolDisplayTypeDeclarationOptions
    {
        None = 0,
        IncludeModifiers = 1,
        IncludeAccessibility = 2,
        BaseType = 4,
        Interfaces = 8,
        BaseList = BaseType | Interfaces
    }
}
