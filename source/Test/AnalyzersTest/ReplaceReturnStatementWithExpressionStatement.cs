// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class ReplaceReturnStatementWithExpressionStatement
    {
        public static void Foo()
        {
            if (true)
                return Foo();

            return Foo();
        }

        public static IEnumerable<object> Foo2()
        {
            if (true)
                yield return Foo();

            yield return Foo();
        }
    }
}
