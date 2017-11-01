// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.MarkLocalVariableAsConst
{
    internal static class MarkLocalVariableAsConstWalkerCache
    {
        [ThreadStatic]
        private static MarkLocalVariableAsConstWalker _cachedInstance;

        public static MarkLocalVariableAsConstWalker Acquire()
        {
            MarkLocalVariableAsConstWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
                walker.Clear();
                return walker;
            }

            return new MarkLocalVariableAsConstWalker();
        }

        public static void Release(MarkLocalVariableAsConstWalker walker)
        {
            _cachedInstance = walker;
        }

        public static HashSet<string> GetAssignedAndRelease(MarkLocalVariableAsConstWalker walker)
        {
            HashSet<string> assigned = walker.Assigned;

            Release(walker);

            return assigned;
        }
    }
}
