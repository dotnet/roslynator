// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analysis.ReturnTaskInsteadOfNull
{
    internal static class ReturnTaskInsteadOfNullWalkerCache
    {
        [ThreadStatic]
        private static ReturnTaskInsteadOfNullWalker _cachedInstance;

        public static ReturnTaskInsteadOfNullWalker GetInstance()
        {
            ReturnTaskInsteadOfNullWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
                walker.Clear();
                return walker;
            }

            return new ReturnTaskInsteadOfNullWalker();
        }

        public static void Free(ReturnTaskInsteadOfNullWalker walker)
        {
            walker.Clear();
            _cachedInstance = walker;
        }
    }
}
