// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1016, RCS1058, RCS1118, RCS1176

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class AvoidBoxingOfValueType
    {
        public static void Bar()
        {
            string s = null;

            object o = null;

            int i = 0;

            int? ni = null;

            var options = RegexOptions.None;

            s = s + i;
            s = s + ni;
            s = s + options;

            s = $"{i}";
            s = $"{ni}";
            s = $"{options}";

            var sb = new StringBuilder();

            sb.Append(options);
            sb.AppendFormat("f", options);
            sb.AppendFormat("f", options, options);
            sb.AppendFormat("f", options, options, options);
            sb.AppendFormat("f", options, options, options, options);
            sb.Insert(0, options);

            //n

            s = s + "";
            s = s + o;
        }
    }
}
