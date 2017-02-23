// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

#pragma warning disable RCS1016, RCS1048, RCS1081, RCS1163

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class UseReadOnlyAutoProperty
    {
        public partial class Foo
        {
            public static string StaticProperty { get; private set; }
            public string Property { get; private set; }
            public static string StaticAssignedInInstanceConstructor { get; private set; }
            public string Assigned { get; private set; }
            public string InSimpleLambda { get; private set; }
            public string InParenthesizedLambda { get; private set; }
            public string InAnonymousMethod { get; private set; }
            public string InLocalFunction { get; private set; }

            static Foo()
            {
                StaticProperty = null;
            }

            public Foo()
            {
                Property = null;
                StaticAssignedInInstanceConstructor = null;
            }

            private void Bar()
            {
                Assigned = null;
            }
        }

        public partial class Foo
        {
            public Foo(object parameter)
            {
                var items = new List<string>();

                IEnumerable<string> q = items.Select(f =>
                {
                    InSimpleLambda = null;
                    return f;
                });

                IEnumerable<string> q2 = items.Select((f) =>
                {
                    InParenthesizedLambda = null;
                    return f;
                });

                IEnumerable<string> q3 = items.Select(delegate(string f)
                {
                    InAnonymousMethod = null;
                    return f;
                });

                LocalFunction();

                void LocalFunction()
                {
                    InLocalFunction = null;
                }
            }
        }
    }
}
