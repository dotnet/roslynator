// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#pragma warning disable RCS1023

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class ReplaceAnyMethodWithCountOrLengthProperty
    {
        private static void Foo()
        {
            var list = new List<string>();

            if (list.Any()) { }

            if (!list.Any()) { }

            var array = new string[0];

            if (array.Any()) { }

            if (!array.Any()) { }

            ImmutableArray<int> ia = ImmutableArray.Create(1);

            if (ia.Any()) { }
        }
    }
}
