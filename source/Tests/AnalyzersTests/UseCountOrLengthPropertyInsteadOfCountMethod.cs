// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

#pragma warning disable RCS1118, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseCountOrLengthPropertyInsteadOfCountMethod
    {
        private static void Foo()
        {
            int x = 0;

            List<object> l = null;
            IList<object> il = null;
            IReadOnlyList<object> irl = null;

            Collection<object> c = null;
            ICollection<object> ic = null;
            IReadOnlyCollection<object> irc = null;

            object[] a = null;

            ImmutableArray<object> ia = ImmutableArray<object>.Empty;

            string s = null;

            x = l.Count();

            x = il.Count();

            x = irl.Count();

            x = c.Count();

            x = ic.Count();

            x = irc.Count();

            x = a.Count();

            x = ia.Count();

            x = s.Count();

            //n

            IEnumerable<object> ie = null;

            x = ie.Count();
        }
    }
}
