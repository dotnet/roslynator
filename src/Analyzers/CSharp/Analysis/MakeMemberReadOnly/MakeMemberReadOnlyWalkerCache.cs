// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analysis.MakeMemberReadOnly
{
    internal static class MakeMemberReadOnlyWalkerCache
    {
        [ThreadStatic]
        private static MakeMemberReadOnlyWalker _cachedInstance;

        public static MakeMemberReadOnlyWalker GetInstance()
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

        public static void Free(MakeMemberReadOnlyWalker walker)
        {
            _cachedInstance = walker;
        }

        public static HashSet<AssignedInfo> GetAssignedAndFree(MakeMemberReadOnlyWalker walker)
        {
            HashSet<AssignedInfo> assigned = walker.Assigned;

            Free(walker);

            return assigned;
        }
    }
}
