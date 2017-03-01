// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

#pragma warning disable CS0219, RCS1016, RCS1021, RCS1048, RCS1054, RCS1090

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RemoveRedundantAsyncAwait
    {
        public static async Task<object> GetAsync()
        {
            return await GetAsync().ConfigureAwait(false);

            async Task<object> GetAsync()
            {
                return await GetAsync().ConfigureAwait(false);
            }
        }

        public static async Task<object> Get2Async()
        {
            return await GetAsync();

            async Task<object> GetAsync()
            {
                return await GetAsync();
            }
        }

        private static void Foo()
        {
            Func<object, Task<object>> func = async f =>
            {
                return await GetAsync().ConfigureAwait(false);
            };

            Func<object, Task<object>> func2 = async (f) =>
            {
                return await GetAsync().ConfigureAwait(false);
            };

            Func<object, Task<object>> func3 = async delegate(object f)
            {
                return await GetAsync().ConfigureAwait(false);
            };
        }

        public static async Task<object> GetValueAsync()
        {
            object value = await GetAsync().ConfigureAwait(false);
            return value;
        }
    }
}
