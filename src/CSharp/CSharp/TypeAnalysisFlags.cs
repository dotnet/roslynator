// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp
{
    [Flags]
    internal enum TypeAnalysisFlags
    {
        None = 0,
        Implicit = 1,
        Explicit = 2,
        Dynamic = 4,
        SupportsImplicit = 8,
        SupportsExplicit = 16,
        TypeObvious = 32,
    }
}
