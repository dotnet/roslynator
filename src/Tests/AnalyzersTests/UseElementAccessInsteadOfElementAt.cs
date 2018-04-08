// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

#pragma warning disable RCS1097, RCS1118, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseElementAccessInsteadOfElementAt
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

            x = l.ElementAt(1);

            x = il.ElementAt(1);

            x = irl.ElementAt(1);

            x = c.ElementAt(1);

            x = a.ElementAt(1);

            x = ia.ElementAt(1);

            x = s.ElementAt(1);

            //n

            x = ic.ElementAt(1);

            x = irc.ElementAt(1);

            x = ie.ElementAt(1);

            KeyValuePair<object, object> kvp = dic.ElementAt(1);

            x = l.ToList().ElementAt(1);

            x = a.ToArray().ElementAt(1);

            x = ia.ToImmutableArray().ElementAt(1);

            x = s.ToUpper().ElementAt(1);
        }
    }
}
