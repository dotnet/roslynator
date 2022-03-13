// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Rename
{
    [Flags]
    internal enum RenameScopeFilter
    {
        None = 0,
        Type = 1,
        Member = 1 << 1,
        Local = 1 << 2,
        All = Type | Member | Local,
    }
}
