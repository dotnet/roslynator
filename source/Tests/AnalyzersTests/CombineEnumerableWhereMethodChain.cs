// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class CombineEnumerableWhereMethodChain
    {
        private static void Foo()
        {
            var items = new List<string>();
            ImmutableArray<string> immutableArray = ImmutableArray.Create<string>();

            IEnumerable<string> q = items.Where(f => true).Where(f => false);

            q = items.Where((f, index) => true).Where((f, index) => false);

            q = immutableArray.Where((f, index) => true).Where((f, index) => false);

            q = immutableArray.Where(f => true).Where(f => false);
        }
    }
}
