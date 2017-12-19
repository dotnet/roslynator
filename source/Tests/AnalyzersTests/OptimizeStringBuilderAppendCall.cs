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

            var __sb = new StringBuilder();

            //sb.Append(s, 0, 2);
            __sb.Append(s.Substring(0, 2));

            //sb.Append(s, 0, 2);
            __sb.Append(s.Remove(2));

            //sb.AppendFormat("f", s);
            __sb.Append(string.Format("f", s));

            //sb.Append(s).Append("s");
            __sb.Append($"{s}s");

            //sb.Append("a").Append(s).Append("b").Append("c").Append(s).Append("d");
            __sb.Append("a" + s + "b").Append("c" + s + "d");

            //sb.Append(s).Append("s");
            __sb.Append(s + "s");

            //sb.Append(s, 0, 2).AppendLine();
            __sb.AppendLine(s.Substring(0, 2));

            //sb.Append(s, 0, 2).AppendLine();
            __sb.AppendLine(s.Remove(2));

            //sb.AppendFormat("f", s).AppendLine();
            __sb.AppendLine(string.Format("f", s));

            //sb.Append(s).AppendLine("s");
            __sb.AppendLine($"{s}s");

            //sb.AppendFormat("{0,1:f}", s).AppendLine();
            __sb.AppendLine($"{s,1:f}");

            //sb.Append("s").Append('s').AppendLine();
            __sb.AppendLine($"s{'s'}");

            //sb.Append("s").Append('s').AppendLine("s");
            __sb.AppendLine($"s{'s'}s");

            //sb.Append("s").AppendLine(s);
            __sb.AppendLine("s" + s);

            //sb.Append("s").Append(i).AppendLine();
            __sb.AppendLine("s" + i);

            //sb.Append("s").Append(o).AppendLine();
            __sb.AppendLine("s" + o);

            //sb.Append("a").Append(s).AppendLine("b").Append("c").Append(s).AppendLine("d");
            __sb.AppendLine("a" + s + "b").AppendLine("c" + s + "d");

            //sb.Append(s).Append("s").Append(s).Append(@"s").Append(s).Append("s").Append(s).Append(@"s").AppendFormat("{0,1}", s).Append("s").AppendFormat("{0:f}", s).Append("s").AppendFormat("{0,1:f}", s).Append("s");
            __sb.Append(s + "s" + s + @"s" + $"{s}s" + $@"{s}s" + $"{s,1}s" + $"{s:f}s" + $"{s,1:f}s");

            //sb.AppendLine("");
            __sb.Append("").AppendLine();

            //sb.AppendLine(s);
            __sb.Append(s).AppendLine();

            //n 

            __sb.Append(s.Substring(2));

            __sb.Append(s.Remove(2, 3));

            __sb.AppendLine(s.Substring(2));

            __sb.AppendLine(s.Remove(2, 3));

            __sb.Insert(0, i);

            __sb.Insert(0, o);

            __sb.Append(i).AppendLine();

            __sb.Append(o).AppendLine();
        }
    }
}
