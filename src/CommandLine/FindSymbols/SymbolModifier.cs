// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.FindSymbols;

[Flags]
internal enum SymbolModifier
{
    None = 0,
    Const = 1,
    Static = 1 << 1,
    Virtual = 1 << 2,
    Sealed = 1 << 3,
    Override = 1 << 4,
    Abstract = 1 << 5,
    ReadOnly = 1 << 6,
}
