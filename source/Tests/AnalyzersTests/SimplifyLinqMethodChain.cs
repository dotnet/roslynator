// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class SimplifyLinqMethodChain
    {
        private static void Foo()
        {
            var items = new List<string>();

            bool x = false;
            int i = 0;
            long l = 0;
            string s = null;

            x = items.Where(f => true).Any();
            i = items.Where(f => true).Count();
            s = items.Where(f => true).First();
            s = items.Where(f => true).FirstOrDefault();
            s = items.Where(f => true).Last();
            s = items.Where(f => true).LastOrDefault();
            l = items.Where(f => true).LongCount();
            s = items.Where(f => true).Single();
            s = items.Where(f => true).SingleOrDefault();
        }

        private static void Foo2()
        {
            ImmutableArray<string> items = ImmutableArray.Create<string>();

            bool x = false;
            int i = 0;
            long l = 0;
            string s = null;

            x = items.Where(f => true).Any();
            i = items.Where(f => true).Count();
            s = items.Where(f => true).First();
            s = items.Where(f => true).FirstOrDefault();
            s = items.Where(f => true).Last();
            s = items.Where(f => true).LastOrDefault();
            l = items.Where(f => true).LongCount();
            s = items.Where(f => true).Single();
            s = items.Where(f => true).SingleOrDefault();
        }

        private static void ReplaceWhereAndCastWithOfType()
        {
            var items = new List<string>();

            IEnumerable<object> q = items.Where(f => f is object).Cast<object>();

            q = items.Where(f =>
            {
                return f is object;
            }).Cast<object>();

            q = items.Where(f => f is string).Cast<object>();
        }
    }
}
