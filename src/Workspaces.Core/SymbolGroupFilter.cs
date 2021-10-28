// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [Flags]
    internal enum SymbolGroupFilter
    {
        None = 0,
        Module = 1,
        Class = 2,
        Delegate = 4,
        Enum = 8,
        Interface = 16,
        Struct = 32,
        Type = Module | Class | Delegate | Enum | Interface | Struct,
        Event = 64,
        Field = 128,
        EnumField = 256,
        Const = 512,
        Method = 1024,
        Property = 2048,
        Indexer = 4096,
        Member = Event | Field | EnumField | Const | Method | Property | Indexer,
        TypeOrMember = Type | Member
    }
}
