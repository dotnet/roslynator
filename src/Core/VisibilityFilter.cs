// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [Flags]
    internal enum VisibilityFilter
    {
        None = 0,
        Public = 1,
        Internal = 1 << 1,
        Private = 1 << 2,
        All = Public | Internal | Private
    }
}
