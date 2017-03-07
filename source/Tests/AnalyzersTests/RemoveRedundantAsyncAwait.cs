// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

#pragma warning disable CS0168, CS0219, RCS1004, RCS1016, RCS1021, RCS1048, RCS1054, RCS1090, RCS1118, RCS1136

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RemoveRedundantAsyncAwait
    {
        public static Task<object> GetAsync() => Task.FromResult(default(object));

        public static async Task<object> MethodWitBodyAsync()
        {
            return await GetAsync().ConfigureAwait(false);

            async Task<object> LocalWithBodyAsync()
            {
                return await GetAsync().ConfigureAwait(false);
            }

            async Task<object> LocalWithExpressionBodyAsync() => await GetAsync().ConfigureAwait(false);
        }

        public static async Task<object> MethodWithExpressionBodyAsync() => await GetAsync().ConfigureAwait(false);

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

            Func<object, Task<object>> func2 = async f => await GetAsync().ConfigureAwait(false);

            Func<object, Task<object>> func3 = async (f) =>
            {
                return await GetAsync().ConfigureAwait(false);
            };

            Func<object, Task<object>> func4 = async (f) => await GetAsync().ConfigureAwait(false);

            Func<object, Task<object>> func5 = async delegate(object f)
            {
                return await GetAsync().ConfigureAwait(false);
            };
        }

        public static async Task<object> IfAndReturnAsync()
        {
            bool f = false;

            if (f)
            {
                return await GetAsync().ConfigureAwait(false);
            }
            else if (f)
            {
                return await GetAsync().ConfigureAwait(false);
            }

            return await GetAsync().ConfigureAwait(false);
        }

        public static async Task<object> IfElseAsync()
        {
            bool f = false;

            if (f)
            {
                return await GetAsync().ConfigureAwait(false);
            }
            else
            {
                return await GetAsync().ConfigureAwait(false);
            }
        }

        public static async Task<object> SwitchAndReturnAsync()
        {
            bool f = false;

            switch (f)
            {
                case true:
                    {
                        return await GetAsync().ConfigureAwait(false);
                    }
                case false:
                    {
                        return await GetAsync().ConfigureAwait(false);
                    }
            }

            return await GetAsync().ConfigureAwait(false);
        }

        public static async Task<object> SwitchWithDefaultAsync()
        {
            bool f = false;

            switch (f)
            {
                case true:
                    {
                        return await GetAsync().ConfigureAwait(false);
                    }
                case false:
                    {
                        return await GetAsync().ConfigureAwait(false);
                    }
                default:
                    {
                        return await GetAsync().ConfigureAwait(false);
                    }
            }
        }

        public static async Task<object> GetValueAsync()
        {
            object value = await GetAsync().ConfigureAwait(false);
            return value;
        }
    }
}
