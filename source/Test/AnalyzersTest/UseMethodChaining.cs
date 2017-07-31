// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

namespace Roslynator.CSharp.Refactorings.Test
{
    internal static class UseMethodChaining
    {
        public static void Foo(string s)
        {
            var sb = new StringBuilder();

            sb.Append("");
            sb.AppendFormat("f", "");
            sb.AppendLine("");
            sb.Clear();
            sb.Insert(0, "");
            sb.Remove(0, 0);
            sb.Replace("", "");

            sb = new StringBuilder();

            sb.Append("1").Append("2");
            sb.Append("3").Append("4");
            sb.Append("5").Append("6");

            sb = new StringBuilder();

            sb.Append("1").Append("2").Append("3");
            sb.Append("4").Append("5").Append("6");
            sb.Append("7").Append("8").Append("9");

            sb = new StringBuilder();

            sb.Append("1");
            sb.Append("2").Append("3");
            sb.Append("4").Append("5").Append("6");
        }
    }
}
