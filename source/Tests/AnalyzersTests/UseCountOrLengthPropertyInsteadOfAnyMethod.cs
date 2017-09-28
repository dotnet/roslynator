// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

#pragma warning disable RCS1023, RCS1118, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseCountOrLengthPropertyInsteadOfAnyMethod
    {
        private static void Foo()
        {
            List<object> l = null;
            IList<object> il = null;
            IReadOnlyList<object> irl = null;

            Collection<object> c = null;
            ICollection<object> ic = null;
            IReadOnlyCollection<object> irc = null;

            object[] a = null;

            string s = null;

            if (l.Any()) { }

            if (!l.Any()) { }

            if (il.Any()) { }

            if (irl.Any()) { }

            if (c.Any()) { }

            if (ic.Any()) { }

            if (irc.Any()) { }

            if (a.Any()) { }

            if (s.Any()) { }

            //n

            ImmutableArray<object> ia = ImmutableArray<object>.Empty;
            IEnumerable<object> ie = null;

            if (ia.Any()) { }

            if (ie.Any()) { }
        }
    }
}
