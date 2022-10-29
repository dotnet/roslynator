// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    public enum SymbolTitleParts
    {
        None = 0,
        ContainingType = 1,
        Parameters = 1 << 1,
        ExplicitImplementation = 1 << 2,
    }
}
