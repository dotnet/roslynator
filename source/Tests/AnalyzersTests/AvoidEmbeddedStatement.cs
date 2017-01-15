// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
#pragma warning disable RCS1002
    internal static class AvoidEmbeddedStatement
    {
        public static void Foo()
        {
            var items = new List<string>();

            foreach (var item in items)
                Foo();

            foreach ((string, string) item in Tuple.Values)
                Foo();
        }
    }
#pragma warning restore RCS1002
}
