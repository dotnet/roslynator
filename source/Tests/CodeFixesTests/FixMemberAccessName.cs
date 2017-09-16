// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    internal static class FixMemberAccessName
    {
        private static void Foo(int x)
        {
            var list = new List<object>();

            x = list.Length;

            x = Items.Length;

            x = GetList().Length;

            var array = new string[0];

            var ia = ImmutableArray.Create(default(object));

            x = array.Count;

            x = ia.Count;
        }

        private static List<object> GetList()
        {
            return null;
        }

        private static Collection<object> Items { get; } = new Collection<object>();
    }
}
