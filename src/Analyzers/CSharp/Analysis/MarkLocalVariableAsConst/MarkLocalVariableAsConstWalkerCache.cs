// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analysis.MarkLocalVariableAsConst
{
    internal static class MarkLocalVariableAsConstWalkerCache
    {
        [ThreadStatic]
        private static MarkLocalVariableAsConstWalker _cachedInstance;

        public static MarkLocalVariableAsConstWalker GetInstance()
        {
            MarkLocalVariableAsConstWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
                walker.Reset();
                return walker;
            }

            return new MarkLocalVariableAsConstWalker();
        }

        public static void Free(MarkLocalVariableAsConstWalker walker)
        {
            _cachedInstance = walker;
        }
    }
}
