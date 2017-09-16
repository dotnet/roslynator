// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class ReplaceStatementWithIfStatementRefactoring
    {
        public static bool Foo(bool value)
        {
            return value;
        }

        public static bool Foo(bool f, bool f2)
        {
            return f || f2;
        }

        public static bool Foo(bool f, bool f2, bool f3)
        {
            return f || f2 || f3;
        }

        public static IEnumerable<bool> Foo2(bool f)
        {
            yield return f;
        }
    }
}
