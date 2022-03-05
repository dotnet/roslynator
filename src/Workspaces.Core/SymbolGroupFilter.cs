// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [Flags]
    internal enum SymbolGroupFilter
    {
        None = 0,
        Module = 1,
        Class = 1 << 1,
        Delegate = 1 << 2,
        Enum = 1 << 3,
        Interface = 1 << 4,
        Struct = 1 << 5,
        Type = Module | Class | Delegate | Enum | Interface | Struct,
        Event = 1 << 6,
        Field = 1 << 7,
        EnumField = 1 << 8,
        Const = 1 << 9,
        Method = 1 << 10,
        Property = 1 << 11,
        Indexer = 1 << 12,
        Member = Event | Field | EnumField | Const | Method | Property | Indexer,
        TypeOrMember = Type | Member
    }
}
