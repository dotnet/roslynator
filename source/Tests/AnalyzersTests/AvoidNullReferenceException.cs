// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

#pragma warning disable RCS1032, RCS1145, RCS1176, RCS1196

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class AvoidNullReferenceException
    {
        private class Foo
        {
            public string Value { get; private set; }

            public static void Bar()
            {
                var items = new List<string>();

                string s = "";

                object o = null;

                char ch = '\0';

                s = items.ElementAtOrDefault(1).ToUpper();
                s = items.FirstOrDefault().ToUpper();
                s = items.LastOrDefault().ToUpper();
                s = (s as string).ToUpper();

                s = ((items.ElementAtOrDefault(1))).ToUpper();
                s = ((items.FirstOrDefault())).ToUpper();
                s = ((items.LastOrDefault())).ToUpper();
                s = (((s as string))).ToUpper();

                s = Enumerable.ElementAtOrDefault(items, 1).ToUpper();
                s = Enumerable.FirstOrDefault(items).ToUpper();
                s = Enumerable.LastOrDefault(items).ToUpper();

                s = ((Enumerable.ElementAtOrDefault(items, 1))).ToUpper();
                s = ((Enumerable.FirstOrDefault(items))).ToUpper();
                s = ((Enumerable.LastOrDefault(items))).ToUpper();

                ch = items.ElementAtOrDefault(1)[0];
                ch = items.FirstOrDefault()[0];
                ch = items.LastOrDefault()[0];
                ch = (s as string)[0];

                ch = ((items.ElementAtOrDefault(1)))[0];
                ch = ((items.FirstOrDefault()))[0];
                ch = ((items.LastOrDefault()))[0];
                ch = (((s as string)))[0];

                ch = Enumerable.ElementAtOrDefault(items, 1)[0];
                ch = Enumerable.FirstOrDefault(items)[0];
                ch = Enumerable.LastOrDefault(items)[0];

                ch = ((Enumerable.ElementAtOrDefault(items, 1)))[0];
                ch = ((Enumerable.FirstOrDefault(items)))[0];
                ch = ((Enumerable.LastOrDefault(items)))[0];

                //no fix

                (o as Foo).Value = s;

                //n

                var values = new List<int>();

                int i = 1;

                i = values.ElementAtOrDefault(1).GetHashCode();
                i = values.FirstOrDefault().GetHashCode();
                i = values.LastOrDefault().GetHashCode();

                i = ((values.ElementAtOrDefault(1))).GetHashCode();
                i = ((values.FirstOrDefault())).GetHashCode();
                i = ((values.LastOrDefault())).GetHashCode();

                s = items.ElementAtOrDefault(1);
                s = items.FirstOrDefault();
                s = items.LastOrDefault();
                s = (s as string);

                s = items.ElementAtOrDefault(1)?.ToUpper();
                s = items.FirstOrDefault()?.ToUpper();
                s = items.LastOrDefault()?.ToUpper();
                s = (s as string)?.ToUpper();

                s = ((items.ElementAtOrDefault(1)))?.ToUpper();
                s = ((items.FirstOrDefault()))?.ToUpper();
                s = ((items.LastOrDefault()))?.ToUpper();
                s = (((s as string)))?.ToUpper();

                s = Enumerable.ElementAtOrDefault(items, 1)?.ToUpper();
                s = Enumerable.FirstOrDefault(items)?.ToUpper();
                s = Enumerable.LastOrDefault(items)?.ToUpper();

                s = ((Enumerable.ElementAtOrDefault(items, 1)))?.ToUpper();
                s = ((Enumerable.FirstOrDefault(items)))?.ToUpper();
                s = ((Enumerable.LastOrDefault(items)))?.ToUpper();

                s = items.SingleOrDefault().ToUpper();
                s = ((items.SingleOrDefault())).ToUpper();
                s = Enumerable.SingleOrDefault(items).ToUpper();
                s = ((Enumerable.SingleOrDefault(items))).ToUpper();

                ch = items.ElementAtOrDefault(1)?[0] ?? default(char);
                ch = items.FirstOrDefault()?[0] ?? default(char);
                ch = items.LastOrDefault()?[0] ?? default(char);
                ch = (s as string)?[0] ?? default(char);

                ch = ((items.ElementAtOrDefault(1)))?[0] ?? default(char);
                ch = ((items.FirstOrDefault()))?[0] ?? default(char);
                ch = ((items.LastOrDefault()))?[0] ?? default(char);
                ch = (((s as string)))?[0] ?? default(char);

                ch = Enumerable.ElementAtOrDefault(items, 1)?[0] ?? default(char);
                ch = Enumerable.FirstOrDefault(items)?[0] ?? default(char);
                ch = Enumerable.LastOrDefault(items)?[0] ?? default(char);

                ch = ((Enumerable.ElementAtOrDefault(items, 1)))?[0] ?? default(char);
                ch = ((Enumerable.FirstOrDefault(items)))?[0] ?? default(char);
                ch = ((Enumerable.LastOrDefault(items)))?[0] ?? default(char);

                var nullables = new List<int?>();

                o = nullables.ElementAtOrDefault(1).ToString();

                o = Enumerable.ElementAtOrDefault(nullables, 1).ToString();

                o = (i as int?).ToString();
            }
        }
    }
}
