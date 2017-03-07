// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Test
{
#pragma warning disable RCS1074
    public static class AvoidStaticMembersInGenericTypes
    {
        public class Foo<T>
        {
            private static string _field = null;

            public static string Field = null;

            public Foo()
            {
            }

            public static void Method()
            {
            }

            public static string Property { get; set; }

            public static event EventHandler Event;

            public static event EventHandler Event2
            {
                add { }
                remove { }
            }
        }
    }
}
