// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Test
{
#pragma warning disable RCS1002, RCS1118
    public static class RemoveRedundantContinueStatement
    {
        private static void Foo()
        {
            bool f = false;

            do
            {
                Foo();
                continue;

            } while (f);

            while (f)
            {
                Foo();
                continue;
            }

            var items = new List<string>();

            for (int i = 0; i < items.Count; i++)
            {
                Foo();
                continue;
            }

            foreach (string item in items)
            {
                Foo();
                continue;
            }

            do
            {
                Foo();

                if (f)
                {
                }
                else
                {
                    continue;
                }

            } while (f);

            while (f)
            {
                Foo();

                if (f)
                {
                }
                else
                {
                    continue;
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                Foo();

                if (f)
                {
                }
                else
                {
                    continue;
                }
            }

            foreach (string item in items)
            {
                Foo();

                if (f)
                {
                }
                else
                {
                    continue;
                }
            }
        }
    }
}
