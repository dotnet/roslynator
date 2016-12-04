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

        public static bool Foo(bool f, bool fTrue, bool fFalse)
        {
            return (f) ? fTrue : fFalse;
        }

        public static bool Foo(bool f, bool fTrue, bool fFalse)
        {
            bool x = false;

            x = (f) ? fTrue : fFalse;
        }

        public static void Foo(bool f, bool fTrue, bool fFalse)
        {
            var x = (f) ? fTrue : fFalse;

            var a = new { Value = 1 };
            var b = new { Value = 2 };

            var s = (f) ? a : b;
        }

        public static IEnumerable<bool> Foo(bool f)
        {
            yield return f;
        }

        public static IEnumerable<bool> Foo(bool f, bool fTrue, bool fFalse)
        {
            yield return (f) ? fTrue : fFalse;
        }
    }
}
