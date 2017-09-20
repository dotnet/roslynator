// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    internal static class RemoveAwaitKeyword
    {
        private static async Task<string> FooTaskOfTAsync()
        {
            return await Foo();
        }

        private static async Task FooTaskAsync()
        {
            await Foo();
        }

        private static string Foo()
        {
            return null;
        }
    }
}
