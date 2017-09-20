// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class FormatConstraintClauses
    {
        private static void Foo<T1>() where
            T1 : class
        {
        }

        private static void Foo<T1, T2, T3>() where T1 : class where T2 : class where T3 : class
        {
        }
    }
}
