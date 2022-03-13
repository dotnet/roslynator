// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [Flags]
    internal enum SymbolDefinitionSortOptions
    {
        None = 0,
        SystemFirst = 1,
        OmitContainingNamespace = 1 << 1,
        OmitContainingType = 1 << 2,
    }
}
