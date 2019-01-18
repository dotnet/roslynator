// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analysis.UnusedMember
{
    internal static class UnusedMemberWalkerCache
    {
        [ThreadStatic]
        private static UnusedMemberWalker _cachedInstance;

        public static UnusedMemberWalker GetInstance()
        {
            UnusedMemberWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
            }
            else
            {
                walker = new UnusedMemberWalker();
            }

            return walker;
        }

        public static void Free(UnusedMemberWalker walker)
        {
            walker.Reset();

            _cachedInstance = walker;
        }
    }
}
