// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.FindSymbols
{
    [Flags]
    internal enum SymbolFlags
    {
        None = 0,
        Const = 1 << 5,
        Static = 1 << 6,
        Virtual = 1 << 7,
        Sealed = 1 << 8,
        Override = 1 << 9,
        Abstract = 1 << 10,
        ReadOnly = 1 << 11,
        Extern = 1 << 12,
        Async = 1 << 14,
        Extension = 1 << 16
    }
}
