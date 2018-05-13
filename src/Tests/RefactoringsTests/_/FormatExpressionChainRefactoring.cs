// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class FormatExpressionChainRefactoring
    {
        private class Foo
        {
            public void Bar()
            {
                var value = new List<string>();

                var items = value.Where(f => f.Length > 0).Select(f => f[0]);
            }

            public void Bar2()
            {
                var value = new List<string>();

                var items = value
                    .Where(f => f.Length > 0)
                    .Select(f => f[0]);
            }

            // n

            public void Bar3()
            {
                var value = new List<string>();

                var items = value
                    .Where(f => f.Length > 0) //x
                    .Select(f => f[0]);
            }
        }
    }
}
