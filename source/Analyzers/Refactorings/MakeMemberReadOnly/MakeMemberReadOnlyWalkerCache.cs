// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings.MakeMemberReadOnly
{
    internal static class MakeMemberReadOnlyWalkerCache
    {
        [ThreadStatic]
        private static MakeMemberReadOnlyWalker _cachedInstance;

        public static MakeMemberReadOnlyWalker Acquire()
        {
            MakeMemberReadOnlyWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
                walker.Clear();
                return walker;
            }

            return new MakeMemberReadOnlyWalker();
        }

        public static void Release(MakeMemberReadOnlyWalker walker)
        {
            _cachedInstance = walker;
        }

        public static HashSet<AssignedInfo> GetAssignedAndRelease(MakeMemberReadOnlyWalker walker)
        {
            HashSet<AssignedInfo> assigned = walker.Assigned;

            Release(walker);

            return assigned;
        }
    }
}
