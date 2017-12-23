// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#pragma warning disable CS0472, RCS1002, RCS1007, RCS1016, RCS1023, RCS1029, RCS1098, RCS1118, RCS1163, RCS1166, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class UseConditionalAccess
    {
        public class Foo
        {
            private const string NullConst = null;
            private const string NonNullConst = "x";

            public string Value { get; }
            public string Value2 { get; }

            public bool IsFoo { get; }

            public void Method()
            {
                string s = null;

                if (s != null &&
                    s.StartsWith("a")) { }

                if (null != s &&
                    s.StartsWith("a")) { }

                if (s != null
                    && s.StartsWith("a") //
                    && s.StartsWith("a")) { }

                if (s != null &&
                    s.Length > 1) { }

                if (s != null &&
                    !s.StartsWith("a")) { }

                if (s != null
                    && !s.StartsWith("a") //
                    && !s.StartsWith("a")) { }

                if (s != null &&
                    (!s.StartsWith("a"))) { }

                Dictionary<int, string> dic = null;

                if (dic != null && dic[0].StartsWith("a")) { }

                if (dic != null && dic[0].Length > 1) { }

                if (dic != null && !dic[0].StartsWith("a")) { }

                Foo x = null;

                if (x != null)
                    x.Method();

                if (x != null)
                {
                    x.Method();
                }

                //y

                if (x != null && x.Value == "x" && x.IsFoo) { }

                if (x != null && x.Value == NonNullConst && x.IsFoo) { }

                if (x != null && x.Value != null && x.IsFoo) { }

                if (x != null && !x.IsFoo && x.IsFoo) { }

                if (x != null && x.Value is object) { }

                if (x != null && x.Value is object y) { }

                if ((x != null) && (x.Value == ("x")) && x.IsFoo) { }

                if ((x != null) && (x.Value == (NonNullConst)) && x.IsFoo) { }

                if ((x != null) && (x.Value != (null)) && x.IsFoo) { }

                //n

                if (x != null && (x.Value == null) is object y2) { }

                if (x != null && x.Value == null && x.IsFoo) { }

                if (x != null && x.Value == NullConst && x.IsFoo) { }

                if (x != null && x.Value == Value && x.IsFoo) { }

                if (x != null && x.Value != "x" && x.IsFoo) { }

                if (x != null && x.Value != NonNullConst && x.IsFoo) { }

                if (x != null && x.Value != Value && x.IsFoo) { }

                int? x2 = null;

                if (x2 != null && x2.HasValue) { }

                string value;
                string result = (dic != null && dic.TryGetValue(0, out value)) ? value : null;

                int i = 0;
                if (i != null && i.ToString() == "0") { }

                DateTime dt = DateTime.MinValue;
                if (dt != null && dt.Ticks == 0) { }
            }

            private struct CustomStruct
            {
                public int Value { get; }

                private void Foo()
                {
                    var x = new CustomStruct();

                    if (x != null && x.Value > 0) { }
                }
            }

            private static void A(object obj)
            {
                B(() => obj != null && obj.GetHashCode() == 0);
            }

            private static void B<T>(Expression<Func<T>> expression)
            {
            }
        }
    }
}
