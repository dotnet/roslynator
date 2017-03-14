// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

#pragma warning disable RCS1023, RCS1029, RCS1118

namespace Roslynator.CSharp.Analyzers.Test
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
                Foo x = null;

                //y

                if (x != null && x.Value == "x" && x.IsFoo) { }

                if (x != null && x.Value == NonNullConst && x.IsFoo) { }

                if (x != null && x.Value != null && x.IsFoo) { }

                if (x != null && !x.IsFoo && x.IsFoo) { }

                //n

                if (x != null && (x.Value == ("x")) && x.IsFoo) { }

                if (x != null && (x.Value == (NonNullConst)) && x.IsFoo) { }

                if (x != null && (x.Value != (null)) && x.IsFoo) { }

                if (x != null && x.Value == null && x.IsFoo) { }

                if (x != null && x.Value == NullConst && x.IsFoo) { }

                if (x != null && x.Value == Value && x.IsFoo) { }

                if (x != null && x.Value != "x" && x.IsFoo) { }

                if (x != null && x.Value != NonNullConst && x.IsFoo) { }

                if (x != null && x.Value != Value && x.IsFoo) { }
            }
        }

        private static void Method()
        {
            string s = null;

            if (s != null &&
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
        }
    }
}
