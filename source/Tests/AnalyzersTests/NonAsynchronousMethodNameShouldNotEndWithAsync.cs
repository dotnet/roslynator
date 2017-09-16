// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using static Roslynator.CSharp.Analyzers.Tests.NonAsynchronousMethodNameShouldNotEndWithAsync;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class NonAsynchronousMethodNameShouldNotEndWithAsync
    {
        public static void FooAsync()
        {
        }

        public static async Task<object> Foo2Async()
        {
            return Foo2Async();
        }
    }

    internal static class NonAsynchronousMethodNameShouldNotEndWithAsync2
    {
        public static void Foo()
        {
            FooAsync();
            Foo2Async();
        }
    }
}
