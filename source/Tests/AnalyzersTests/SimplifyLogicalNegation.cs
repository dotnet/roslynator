// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable CS0660, CS0661, RCS1016, RCS1032, RCS1060, RCS1079, RCS1102, RCS1118, RCS1163, RCS1175, RCS1176

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public class SimplifyLogicalNegation
    {
        private static void Bar()
        {
            bool f = false;
            bool f2 = false;

            f = !true;
            f = !false;
            f = !(true);
            f = !(false);

            f = !!(f2);
            f = !(!(f2));

            f = !(f == f2);
            f = !((f == f2));

            var items = new List<string>();

            f = !items.Any(s => !s.Equals(s));
            f = !(items.Any(s => (!s.Equals(s))));
            f = !items.Any<string>(s => !s.Equals(s));

            f = !items.All(s => !s.Equals(s));
            f = !(items.All(s => (!s.Equals(s))));
            f = !items.All<string>(s => !s.Equals(s));
        }

        //n

        private class Foo
        {
            public static bool operator ==(Foo left, Foo right)
            {
                return false;
            }

            public static bool operator !=(Foo left, Foo right)
            {
                return !(left == right);
            }
        }
    }
}
