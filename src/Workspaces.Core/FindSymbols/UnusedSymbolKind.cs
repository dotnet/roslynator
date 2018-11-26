// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.FindSymbols
{
    internal enum UnusedSymbolKind
    {
        None = 0,
        Class = 1,
        Delegate = 2,
        Enum = 4,
        Event = 8,
        Field = 16,
        Interface = 32,
        Method = 64,
        Property = 128,
        Struct = 256,
    }
}
