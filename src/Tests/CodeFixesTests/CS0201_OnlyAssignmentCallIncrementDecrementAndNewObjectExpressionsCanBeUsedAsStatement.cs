// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    internal static class CS0201_OnlyAssignmentCallIncrementDecrementAndNewObjectExpressionsCanBeUsedAsStatement
    {
        private class Foo
        {
            private void Bar()
            {
                DateTime _dateTime = DateTime.Now;

                DateTime.Now;

                var foo = new Foo();

                Bar;
                foo.Bar;

                bool condition = false;
                (condition) ? Bar() : Bar2();
            }

            public void Bar2()
            {
            }

            private static void StaticBar()
            {
                DateTime _dateTime = DateTime.Now;

                DateTime.Now;
            }
        }

        private class ReplaceComparisonWithAssignment
        {
            public void Bar(string value)
            {
                value == null;

                value == "";
            }
        }
    }
}
