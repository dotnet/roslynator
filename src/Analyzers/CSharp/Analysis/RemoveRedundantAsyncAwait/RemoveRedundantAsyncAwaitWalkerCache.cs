// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis.RemoveRedundantAsyncAwait
{
    internal static class RemoveRedundantAsyncAwaitWalkerCache
    {
        [ThreadStatic]
        private static RemoveRedundantAsyncAwaitWalker _cachedInstance;

        public static RemoveRedundantAsyncAwaitWalker GetInstance(TextSpan span, bool stopOnFirstAwaitExpression = false)
        {
            RemoveRedundantAsyncAwaitWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
                walker.Clear();
            }
            else
            {
                walker = new RemoveRedundantAsyncAwaitWalker();
            }

            walker.SetValues(span: span, stopOnFirstAwaitExpression: stopOnFirstAwaitExpression);

            return walker;
        }

        public static void Free(RemoveRedundantAsyncAwaitWalker walker)
        {
            walker.Clear();
            _cachedInstance = walker;
        }
    }
}
