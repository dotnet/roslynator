// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analyzers.UnnecessaryUnsafeContext
{
    internal static class UnnecessaryUnsafeContextWalkerCache
    {
        [ThreadStatic]
        private static UnnecessaryUnsafeContextWalker _cachedInstance;

        public static UnnecessaryUnsafeContextWalker GetInstance()
        {
            UnnecessaryUnsafeContextWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
                walker.Reset();
            }
            else
            {
                walker = new UnnecessaryUnsafeContextWalker();
            }

            return walker;
        }

        public static void Free(UnnecessaryUnsafeContextWalker walker)
        {
            _cachedInstance = walker;
        }
    }
}
