// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analysis.ReduceIfNesting
{
    internal static class ReduceIfNestingOptionsExtensions
    {
        public static bool AllowNestedFix(this ReduceIfNestingOptions options)
        {
            return (options & ReduceIfNestingOptions.AllowNestedFix) != 0;
        }

        public static bool AllowLoop(this ReduceIfNestingOptions options)
        {
            return (options & ReduceIfNestingOptions.AllowLoop) != 0;
        }

        public static bool AllowSwitchSection(this ReduceIfNestingOptions options)
        {
            return (options & ReduceIfNestingOptions.AllowSwitchSection) != 0;
        }

        public static bool AllowIfInsideIfElse(this ReduceIfNestingOptions options)
        {
            return (options & ReduceIfNestingOptions.AllowIfInsideIfElse) != 0;
        }
    }
}
