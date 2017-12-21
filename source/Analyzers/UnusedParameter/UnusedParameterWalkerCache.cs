// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Analyzers.UnusedParameter
{
    internal static class UnusedParameterWalkerCache
    {
        [ThreadStatic]
        private static UnusedParameterWalker _cachedInstance;

        public static UnusedParameterWalker Acquire(SemanticModel semanticModel, CancellationToken cancellationToken, bool isIndexer = false)
        {
            UnusedParameterWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
                walker.Reset();
            }
            else
            {
                walker = new UnusedParameterWalker();
            }

            walker.SemanticModel = semanticModel;
            walker.CancellationToken = cancellationToken;
            walker.IsIndexer = isIndexer;

            return walker;
        }

        public static void Release(UnusedParameterWalker walker)
        {
            _cachedInstance = walker;
        }

        public static Dictionary<string, NodeSymbolInfo> GetNodesAndRelease(UnusedParameterWalker walker)
        {
            Dictionary<string, NodeSymbolInfo> nodes = walker.Nodes;

            Release(walker);

            return nodes;
        }
    }
}

