// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using static Roslynator.CSharp.Analyzers.Tests.AsynchronousMethodNameShouldEndWithAsync;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class AsynchronousMethodNameShouldEndWithAsync
    {
        public static void FooAsync()
        {
        }

        public static async Task<bool> Foo()
        {
            bool f = false;
            return await Task.FromResult(false).ConfigureAwait(f);
        }

        public static async Task<bool> Foo2()
        {
            bool f = false;
            return await Task.FromResult(false).ConfigureAwait(f);
        }

        public static async Task<bool> Bar()
        {
            bool f = false;
            return await Task.FromResult(false).ConfigureAwait(f);
        }
    }

    internal static class AsynchronousMethodNameShouldEndWithAsync2
    {
        public static void Foo2Async()
        {
            Foo();
            Foo2();
            Bar();
        }
    }
}
