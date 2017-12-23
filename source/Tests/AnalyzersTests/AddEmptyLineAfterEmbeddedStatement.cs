// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class AddEmptyLineAfterEmbeddedStatement
    {
        private static void Foo()
        {
            bool f = false;
            var items = new List<object>();

            if (f)
                Foo();
            if (f)
                Foo();
            else
                Foo();
            foreach (object item in items)
                Foo();
            foreach ((string, string) item in Tuple.Values)
                Foo();
            for (int i = 0; i < items.Count; i++)
                Foo();
            using ((IDisposable)null)
                Foo();
            while (f)
                Foo();
            do
                Foo();
            while (f);
            lock (null)
                Foo();
            unsafe
            {
                fixed ()
                    Foo();
                Foo();
            }

            /**/

            if (f) Foo();
            if (f) Foo(); else Foo();
            if (f)
                Foo();
            else Foo();
            foreach (object item in items) Foo();
            for (int i = 0; i < items.Count; i++) Foo();
            using ((IDisposable)null) Foo();
            while (f) Foo();
            do Foo();
            while (f);
            lock (null) Foo();
            unsafe
            {
                fixed () Foo();
            }

            /**/

            if (f)
            {
                Foo();
            }
            else
            {
                Foo();
            }

            foreach (object item in items)
            {
                Foo();
            }

            for (int i = 0; i < items.Count; i++)
            {
                Foo();
            }

            using ((IDisposable)null)
            {
                Foo();
            }

            while (f)
            {
                Foo();
            }

            do
            {
                Foo();
            }
            while (f);

            lock (null)
            {
                Foo();
            }

            unsafe
            {
                fixed ()
                {
                    Foo();
                }
            }
        }
    }
}
