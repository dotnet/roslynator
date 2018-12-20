// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.FindSymbols
{
    internal enum UnusedSymbolKind
    {
        None = 0,
        Class = 1,
        Delegate = 2,
        Enum = 3,
        Event = 4,
        Field = 5,
        Interface = 6,
        Method = 7,
        Property = 8,
        Struct = 9
    }
}
