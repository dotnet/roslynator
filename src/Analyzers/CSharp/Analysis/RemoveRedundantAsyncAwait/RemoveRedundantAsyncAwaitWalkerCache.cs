// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analysis.RemoveRedundantAsyncAwait
{
    internal static class RemoveRedundantAsyncAwaitWalkerCache
    {
        [ThreadStatic]
        private static RemoveRedundantAsyncAwaitWalker _cachedInstance;

        public static RemoveRedundantAsyncAwaitWalker GetInstance()
        {
            RemoveRedundantAsyncAwaitWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
                walker.Reset();
                return walker;
            }
            else
            {
                return new RemoveRedundantAsyncAwaitWalker();
            }
        }

        public static void Free(RemoveRedundantAsyncAwaitWalker walker)
        {
            walker.Reset();
            _cachedInstance = walker;
        }
    }
}
