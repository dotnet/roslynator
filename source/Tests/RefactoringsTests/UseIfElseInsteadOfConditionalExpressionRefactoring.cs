// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class UseIfElseInsteadOfConditionalExpressionRefactoring
    {
        public static bool Foo()
        {
            bool condition = false;
            bool fTrue = false;
            bool fFalse = false;

            var x = (condition) ? fTrue : fFalse;

            x = (condition) ? fTrue : fFalse;

            string s = null;

            s += (condition) ? "true" : "false";

            return (condition) ? fTrue : fFalse;
        }

        public static IEnumerable<bool> FooIterator()
        {
            bool condition = false;
            bool fTrue = false;
            bool fFalse = false;

            yield return (condition) ? fTrue : fFalse;
        }

        //n

        public static void FooAnonymousType()
        {
            bool condition = false;

            var x = (condition) ? (new { Value = 1 }) : (new { Value = 2 });
        }
    }
}
