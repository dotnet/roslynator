// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#pragma warning disable CS0472, RCS1002, RCS1007, RCS1016, RCS1023, RCS1029, RCS1097, RCS1098, RCS1118, RCS1163, RCS1166, RCS1176, RCS1208

namespace Roslynator.CSharp.Analyzers.Tests
{
    namespace IfStatement
    {
        public class C
        {
            public void M()
            {
                C x = null;

                if (x != null)
                    x.M();

                if (x != null)
                {
                    x.M();
                }
            }
        }

        public struct S
        {
            public void M()
            {
                S? x = null;

                if (x != null)
                    x.Value.M();

                if (x != null)
                {
                    x.Value.M();
                }
            }
        }
    }

    namespace Diagnostic_ReferenceType
    {
        public class Foo
        {
            private const string NonNullConst = "x";

            public string Value { get; }

            public void M()
            {
                bool f = false;

                Foo x = null;

                if (x != null && x.Equals(x)) { }

                if (null != x && x.Equals(x)) { }

                if (x != null && (x.Equals(x))) { }

                if (x != null && x.Equals(x) && f) { }

                if (f && x != null && x.Equals(x)) { }

                if (x != null && x.Value.Length > 1) { }

                if (x != null && !x.Equals(x)) { }

                if (x != null && (!x.Equals(x))) { }

                if (x != null && x.Value == "x") { }

                if (x != null && x.Value == NonNullConst) { }

                if (x != null && x.Value != null) { }

                if (x != null && x.Value is object) { }

                if (x != null && x.Value is object _) { }

                if (x != null && x.ToString() != null && x.ToString().ToString() != null) { }

                if (f &&
        /*leading*/ x != null &&
                    x.Equals("x") /*trailing*/
                    && f) { }

                Dictionary<int, string> dic = null;

                if (dic != null && dic[0].Equals("x")) { }

                if (dic != null && dic[0].Length > 1) { }

                if (dic != null && !dic[0].Equals("x")) { }
            }
        }
    }

    namespace Diagnostic_NullableType
    {
        public struct Foo
        {
            private const string NonNullConst = "x";

            public string V { get; }

            public void M()
            {
                Foo? x = null;

                if (x != null && x.Value.Equals(x)) { }

                if (x != null && x.Value.V.Length > 1) { }

                if (x != null && !x.Value.Equals(x)) { }

                if (x != null && x.Value.V == "x") { }

                if (x != null && x.Value.V == NonNullConst) { }

                if (x != null && x.Value.V != null) { }

                if (x != null && x.Value.V is object) { }

                if (x != null && x.Value.V is object _) { }

                if (x != null && x.Value.ToString() != null && x.Value.ToString().ToString() != null) { }
            }
        }
    }

    //n

    namespace NoDiagnostic_ReferenceType
    {
        public class Foo
        {
            private const string NullConst = null;
            private const string NonNullConst = "x";

            public string Value { get; }

            public bool Is { get; }

            public void M()
            {
                bool f = false;

                string s = null;

                Foo x = null;

                if (x != null && x.Value == null && f) { }

                if (x != null && x.Value == NullConst && f) { }

                if (x != null && x.Value == s && f) { }

                if (x != null && x.Value != "x" && f) { }

                if (x != null && x.Value != NonNullConst && f) { }

                if (x != null && x.Value != s && f) { }

                if (x != null && (x.Value != null) is object _) { }
            }
        }
    }

    namespace NoDiagnostic_ValueType
    {
        public struct S
        {
            public int Value { get; }

            public void M()
            {
                int i = 0;
                if (i != null && i.ToString() == "x") { }

                DateTime dt = DateTime.MinValue;
                if (dt != null && dt.Ticks == 0) { }

                var x = new S();

                if (x != null && x.Value > 0) { }
            }

            public static bool operator ==(S left, S right) => left.Equals(right);

            public static bool operator !=(S left, S right) => !(left == right);
        }
    }

    namespace NoDiagnostic_NullableType
    {
        public struct Foo
        {
            public bool B { get; }

            public void M()
            {
                bool? b = null;

                if (b != null && b.Value) { }

                if (b != null && b.Value && b.Value) { }

                if (b != null && (b.Value)) { }

                if (b != null && !b.Value) { }

                if (b != null && !b.Value && !b.Value) { }

                if (b != null && (!b.Value)) { }

                Foo? f = null;

                Foo value = default;

                if (f != null && f.Value == null && f.Value.B) { }

                if (f != null && f.Value == value && f.Value.B) { }

                if (f != null && f.Value != null && f.Value.B) { }

                if (f != null && f.Value != null && f.Value.B) { }

                if (f != null && f.Value != value && f.Value.B) { }

                if (f != null && f.HasValue.Equals(true)) { }

                if (f != null && (f.Value == null) is object _) { }
            }

            public static bool operator ==(Foo left, Foo right) => left.Equals(right);

            public static bool operator !=(Foo left, Foo right) => !(left == right);
        }
    }

    namespace NoDiagnostic_OutParameter
    {
        public class C
        {
            public void M()
            {
                Dictionary<int, string> dic = null;

                string value;
                if (dic != null && dic.TryGetValue(0, out value))
                {
                }

                if (dic != null && dic.TryGetValue(0, out string value2))
                {
                }
            }
        }
    }

    namespace NoDiagnostic_ExpressionTree
    {
        public class C
        {
            public void M<T>(Expression<Func<T>> expression)
            {
                string s = null;

                M(() => s != null && s.GetHashCode() == 0);
            }
        }
    }
}
