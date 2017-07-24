// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

namespace Roslynator.CSharp.Refactorings.Test
{
    internal static class OptimizeStringBuilderAppendCall
    {
        public static void Foo(string s)
        {
            var sb = new StringBuilder();

            //a
            sb.Append(s.Substring(0, 2)); //b

            //a
            sb.Append(s.Remove(2)); //b

            //a
            sb.Append(string.Format("f", s)); //b

            //a
            sb.Append($"{s}s"); //b

            //a
            sb.Append(s + "s"); //b

            //a
            sb.AppendLine(s.Substring(0, 2)); //b

            //a
            sb.AppendLine(s.Remove(2)); //b

            //a
            sb.AppendLine(string.Format("f", s)); //b

            //a
            sb.AppendLine($"{s}s"); //b

            //a
            sb.AppendLine(s + "s"); //b

            //a
            sb.Append(s + "s" + s + @"s" + $"{s}s" + $@"{s}s" + $"{s,1}s" + $"{s:f}s" + $"{s,1:f}s"); //b

            //n 

            sb.Append(s.Substring(2));

            sb.Append(s.Remove(2, 3));

            sb.AppendLine(s.Substring(2));

            sb.AppendLine(s.Remove(2, 3));
        }
    }
}
