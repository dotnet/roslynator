// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.FindSymbols
{
    [Flags]
    internal enum SymbolFlags
    {
        None = 0,
        Const = 32,
        Static = 64,
        Virtual = 128,
        Sealed = 256,
        Override = 512,
        Abstract = 1024,
        ReadOnly = 2048,
        Extern = 4096,
        Async = 16384,
        Extension = 65536
    }
}
