// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [Flags]
    internal enum SymbolGroupFilter
    {
        None = 0,
        Class = 1,
        Delegate = 2,
        Enum = 4,
        Interface = 8,
        Struct = 16,
        Type = Class | Delegate | Enum | Interface | Struct,
        Event = 32,
        Field = 64,
        EnumField = 128,
        Const = 256,
        Method = 512,
        Property = 1024,
        Indexer = 2048,
        Member = Event | Field | EnumField | Const | Method | Property | Indexer,
        TypeOrMember = Type | Member
    }
}
