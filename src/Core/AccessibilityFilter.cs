// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [Flags]
    internal enum AccessibilityFilter
    {
        None = 0,
        Private = 1,
        ProtectedAndInternal = 1 << 1,
        Protected = 1 << 2,
        Internal = 1 << 3,
        ProtectedOrInternal = 1 << 4,
        Public = 1 << 5,
    }
}
