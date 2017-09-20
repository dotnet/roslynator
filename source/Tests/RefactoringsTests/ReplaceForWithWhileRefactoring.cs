// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class ReplaceForWithWhileRefactoring
    {
        public static void Foo()
        {
            var items = new List<string>();

            for (int i = 0; i < items.Count; i++)
            {
                items[i] = null;
            }

            string x = null;
            string y = null;

            for (x = ""; x != null; x = x.ToLower())
            {
                x = x.ToUpper();
            }

            for (x = "", y = ""; x != null; x = x.ToLower(), y = y.ToLower())
            {
                x = x.ToUpper();
            }

            for (;;)
            {

            }

            while (true)
                for (int i = 0; i < items.Count; i++)
                {
                    items[i] = null;
                }
        }
    }
}
