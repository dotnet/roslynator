// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#pragma warning disable RCS1021, RCS1176

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
            s = items.Where(f => true)
                .SingleOrDefault();
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
            s = items.Where(f => true)
                .SingleOrDefault();
        }

        private static void CallOfTypeInsteadOfWhereAndCast()
        {
            var items = new List<string>();

            IEnumerable<object> q = items.Where(f => f is object).Cast<object>();
            IEnumerable<object> q2 = items.Where((f) => f is object).Cast<object>();

            q = items.Where(f =>
            {
                return f is object;
            }).Cast<object>();

            q = items.Where(f => f is string).Cast<object>();
        }

        private static void CombineWhereAndAny()
        {
            var items = new List<string>();
            ImmutableArray<string> ia = ImmutableArray<string>.Empty;

            bool any = items.Where((f) => f.StartsWith("a")).Any(f => f.StartsWith("b"));
            bool any2 = ia.Where(f => f.StartsWith("a")).Any((f) => f.StartsWith("b"));

            //n

            bool any3 = items.Where(f => f.StartsWith("a")).Any(g => g.StartsWith("b"));
        }
    }
}
