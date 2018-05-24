// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

#pragma warning disable RCS1097, RCS1118, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseElementAccessInsteadOfFirst
    {
        public static void Foo()
        {
            object x = 0;

            List<object> l = null;
            IList<object> il = null;
            IReadOnlyList<object> irl = null;
            Collection<object> c = null;
            ICollection<object> ic = null;
            IReadOnlyCollection<object> irc = null;
            IEnumerable<object> ie = null;
            object[] a = null;
            ImmutableArray<object> ia = ImmutableArray<object>.Empty;
            string s = null;
            var dic = new Dictionary<object, object>();

            x = l.First();

            x = il.First();

            x = irl.First();

            x = c.First();

            x = a.First();

            x = ia.First();

            x = s.First();

            //n

            x = ic.First();

            x = irc.First();

            x = ie.First();

            KeyValuePair<object, object> kvp = dic.First();

            x = l.ToList().First();

            x = a.ToArray().First();

            x = ia.ToImmutableArray().First();

            x = s.ToUpper().First();
        }
    }
}
