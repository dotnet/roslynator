// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

#pragma warning disable RCS1002

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class InlineLazyInitialization
    {
        private static void Foo()
        {
            List<string> items = null;

            Foo();

            if (items == null)
            {
                items = new List<string>();
            }

            items.Add("");
        }

        private static void Foo2()
        {
            List<string> items = null;

            Foo();

            // lazy
            if (items == null)
                items = new List<string>();

            items.Add("");
        }
    }
}
