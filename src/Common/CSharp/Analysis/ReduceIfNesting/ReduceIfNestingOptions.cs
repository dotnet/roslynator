// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analysis.ReduceIfNesting
{
    [Flags]
    internal enum ReduceIfNestingOptions
    {
        None = 0,
        AllowNestedFix = 1,
        AllowIfInsideIfElse = 2,
        AllowLoop = 4,
        AllowSwitchSection = 8,
    }
}
