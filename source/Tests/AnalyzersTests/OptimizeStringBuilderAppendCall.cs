// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;

#pragma warning disable RCS1118, RCS1176, RCS1192, RCS1198, RCS1201

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class OptimizeStringBuilderAppendCall
    {
        public static void Foo()
        {
            string s = null;
            int i = 0;
            object o = null;

            var sb = new StringBuilder();

            //a
            sb.Append(s.Substring(0, 2)); //b

            sb.Append(s.Remove(2));

            sb.Append(string.Format("f", s));

            sb.Append($"{s}s");

            sb.Append("a" + s + "b").Append("c" + s + "d");

            sb.Append(s + "s");

            sb.AppendLine(s.Substring(0, 2));

            sb.AppendLine(s.Remove(2));

            sb.AppendLine(string.Format("f", s));

            sb.AppendLine($"{s}s");

            sb.AppendLine($"{s,1:f}");

            sb.AppendLine($"s{'s'}");

            sb.AppendLine($"s{'s'}s");

            sb.AppendLine("s" + s);

            sb.AppendLine("s" + i);

            sb.AppendLine("s" + o);

            sb.AppendLine("a" + s + "b").AppendLine("c" + s + "d");

            sb.Append(s + "s" + s + @"s" + $"{s}s" + $@"{s}s" + $"{s,1}s" + $"{s:f}s" + $"{s,1:f}s");

            //n 

            sb.Append(s.Substring(2));

            sb.Append(s.Remove(2, 3));

            sb.AppendLine(s.Substring(2));

            sb.AppendLine(s.Remove(2, 3));
        }
    }
}
