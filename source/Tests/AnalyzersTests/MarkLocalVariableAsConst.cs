// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

#pragma warning disable CS0219, RCS1008, RCS1078, RCS1081, RCS1124, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class MarkLocalAsConst
    {
        public static void Foo()
        {
            string s = "";
            string x = s;

            string s2 = s + "";
            string x2 = s2;

            var s3 = "";
            string x3 = s3;

            string s4 = "", s5 = "";
            string x4 = s4;
            string x5 = s5;

            bool f = false;
            bool f2 = f;

            var options = StringSplitOptions.None;
            StringSplitOptions options2 = options;

            // n

            string s6 = "", s7 = string.Empty;
            string x6 = s6;
            string x7 = s7;

            string s8 = string.Empty;
            string x8 = s8;

            string s9 = "";
            string x9 = s9;
            s9 = null;

            int i = 0;
            if (int.TryParse("", out i))
            {
            }

            string tuple1 = null;
            string tuple2 = null;

            (tuple1, tuple2) = default((string, string));
        }
    }
}
