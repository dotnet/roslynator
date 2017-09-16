// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable RCS1176

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class UseMethodChaining
    {
        public static void Foo()
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

            sb.Append("1");
            sb.Append("2").Append("3");
            sb.Append("4").Append("5").Append("6");

            sb = new StringBuilder();

            sb = sb.Append("1");
            sb = sb.Append("2").Append("3");
            sb = sb.Append("4").Append("5").Append("6");

            sb = new StringBuilder();

            sb.Append("1");
            sb.Append(sb);

            IEnumerable<object> q = Enumerable.Empty<object>();

            q = q.Select(f => "1");

            q = q.Select(f => "2")
                .Select(f => "3");

            q = q.Select(f => "4")
                .Select(f => "5")
                .Select(f => "6");

            //n

            sb = new StringBuilder();

            sb = sb.Append("1");
            sb.Append("2");

            q = Enumerable.Empty<object>();

            q = q.Select(f => f);
            q = q.Select(f => (object)q);

            q = Enumerable.Empty<object>();

            q.Select(f => "1");
            q.Select(f => "2");
        }
    }
}
