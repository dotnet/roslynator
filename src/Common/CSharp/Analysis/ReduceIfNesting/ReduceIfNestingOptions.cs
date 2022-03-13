// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analysis.ReduceIfNesting
{
    [Flags]
    internal enum ReduceIfNestingOptions
    {
        None = 0,
        AllowNestedFix = 1,
        AllowIfInsideIfElse = 1 << 1,
        AllowLoop = 1 << 2,
        AllowSwitchSection = 1 << 3,
    }
}
