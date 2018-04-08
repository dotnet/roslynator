// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class ReplaceWhileWithForRefactoring
    {
        public static void Foo()
        {
            var items = new List<string>();

            int i = 0;
            while (i < items.Count)
            {
                items[i] = null;
                i++;
            }

            xxx j = 0;
            while (j < items.Count)
            {
                items[j] = null;
            }

            int k = 0;

            Foo();
            Foo();
            while (k < items.Count)
            {
                items[k] = null;
                k++;
            }
        }

        public static void Foo2()
        {
            var items = new List<string>();

            int i = 0;
            int j = 0;
            while (i < items.Count && j < items.Count)
            {
                items[i] = null;
                i++;
                j++;
            }
        }

        //n

        public static void Foo3()
        {
            var items = new List<string>();

            int j = 0;
            Foo();
            while (j < items.Count)
            {
                items[j] = null;
                j++;
            }
        }

        public static void Foo4()
        {
            var items = new List<string>();

            int i = 0;
            long j = 0;
            while (i < items.Count)
            {
                items[i] = null;
                i++;
            }
        }

        public static void Foo5()
        {
            var items = new List<string>();

            int i = 0;
            while (i < items.Count)
            {
                items[i] = null;
                i++;
            }

            i = 0;
        }

        public static void Foo6()
        {
            var items = new List<string>();

            Foo();
            int i = 0;
            while (i < items.Count)
            {
                items[i] = null;
                i++;
            }
        }

        public static void Foo7()
        {
            int i = 0;

            var items = new List<string>();

            Foo();
            i += 2;
            while (i < items.Count)
            {
                items[i] = null;
                i++;
            }
        }
    }
}
