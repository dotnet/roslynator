// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Analysis.UnusedMember
{
    internal static class UnusedMemberWalkerCache
    {
        [ThreadStatic]
        private static UnusedMemberWalker _cachedInstance;

        public static UnusedMemberWalker GetInstance(SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            UnusedMemberWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
                walker.Reset();
            }
            else
            {
                walker = new UnusedMemberWalker();
            }

            walker.SemanticModel = semanticModel;
            walker.CancellationToken = cancellationToken;

            return walker;
        }

        public static void Free(UnusedMemberWalker walker)
        {
            _cachedInstance = walker;
        }

        public static Collection<NodeSymbolInfo> GetNodesAndFree(UnusedMemberWalker walker)
        {
            Collection<NodeSymbolInfo> nodes = walker.Nodes;

            Free(walker);

            return nodes;
        }
    }
}
