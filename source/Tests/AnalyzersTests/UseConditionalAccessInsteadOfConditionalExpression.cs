// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#pragma warning disable RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class UseConditionalAccessInsteadOfConditionalExpression
    {
        public class Foo
        {
            public int Value { get; }

            private static void Bar()
            {
                var x = new Foo();
                var x2 = new Foo();

                string s = null;
                int i = 0;
                int? ni = null;
                int? ni2 = null;

                i = (x != null) ? x.Value : default(int);
                i = (x == null) ? default(int) : x.Value;

                i = (x != null) ? x.Value : 0;
                i = (x == null) ? 0 : x.Value;

                s = (x != null) ? x.ToString() : null;
                s = (x == null) ? null : x.ToString();

                s = (x != null) ? x.ToString() : default(string);
                s = (x == null) ? default(string) : x.ToString();

                i = (ni != null) ? ni.Value.GetHashCode() : 0;
                i = (ni == null) ? 0 : ni.Value.GetHashCode();

                i = (ni.HasValue) ? ni.Value.GetHashCode() : 0;
                i = (!ni.HasValue) ? 0 : ni.Value.GetHashCode();

                s = (ni != null) ? ni.Value.ToString() : null;
                s = (ni == null) ? null : ni.Value.ToString();

                s = (ni.HasValue) ? ni.Value.ToString() : null;
                s = (!ni.HasValue) ? null : ni.Value.ToString();

                //n

                i = (x != null) ? x2.Value : default(int);
                i = (x == null) ? default(int) : x2.Value;

                i = (x != null) ? x.Value : 1;
                i = (x == null) ? 1 : x.Value;

                i = (ni != null) ? ni2.Value : default(int);
                i = (ni == null) ? default(int) : ni2.Value;

                i = (ni.HasValue) ? ni2.Value : default(int);
                i = (!ni.HasValue) ? default(int) : ni2.Value;

                i = (ni != null) ? ni.Value : 1;
                i = (ni == null) ? 1 : ni.Value;

                i = (ni.HasValue) ? ni.Value : 1;
                i = (!ni.HasValue) ? 1 : ni.Value;

                s = (i != null) ? i.ToString() : null;
                s = (i == null) ? null : i.ToString();
            }
        }
    }
}
