// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [Flags]
    internal enum TypeAnalysisFlags
    {
        None = 0,
        Implicit = 1,
        Explicit = 1 << 1,
        Dynamic = 1 << 2,
        SupportsImplicit = 1 << 3,
        SupportsExplicit = 1 << 4,
        TypeObvious = 1 << 5,
    }
}
