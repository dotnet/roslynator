// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analysis.UsePatternMatching
{
    internal static class UsePatternMatchingWalkerCache
    {
        [ThreadStatic]
        private static UsePatternMatchingWalker _cachedInstance;

        public static UsePatternMatchingWalker GetInstance()
        {
            UsePatternMatchingWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
                walker.Clear();
                return walker;
            }

            return new UsePatternMatchingWalker();
        }

        public static void Free(UsePatternMatchingWalker walker)
        {
            walker.Clear();
            _cachedInstance = walker;
        }

        public static bool GetIsFixableAndFree(UsePatternMatchingWalker walker)
        {
            bool? isFixable = walker.IsFixable;

            Free(walker);

            return isFixable == true;
        }
    }
}

