// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings.Test
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
    }
}
