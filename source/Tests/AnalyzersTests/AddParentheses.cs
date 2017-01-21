// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class AddParentheses
    {
        private static void Foo()
        {
            const bool f = false;
            const bool f2 = false;
            const bool f3 = false;

            if (f || f2 && f3)
            {
            }

            if (f && f2 || f3)
            {
            }

            const int x = 1 * 1 - 1 * 1;
        }
    }
}
