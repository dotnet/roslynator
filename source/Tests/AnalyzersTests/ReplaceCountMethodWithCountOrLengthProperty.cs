// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class ReplaceCountMethodWithCountOrLengthProperty
    {
        private static void Foo()
        {
            var list = new List<string>();

            var array = new string[0];

            ImmutableArray<string> immutableArray = ImmutableArray.Create<string>();

            int count = list.Count();

            count = array.Count();

            count = immutableArray.Count();
        }
    }
}
