// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.FindSymbols;

[Flags]
internal enum SymbolModifier
{
    None = 0,
    Const = 1 << 5,
    Static = 1 << 6,
    Virtual = 1 << 7,
    Sealed = 1 << 8,
    Override = 1 << 9,
    Abstract = 1 << 10,
    ReadOnly = 1 << 11,
    Async = 1 << 12,
}
