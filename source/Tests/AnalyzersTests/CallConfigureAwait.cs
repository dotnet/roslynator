// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class CallConfigureAwait
    {
        public static async Task GetValueAsync()
        {
            await GetValueAsync();
            object result = await GetValueAsync2();

            await GetValue();
            await GetValueAsync().ConfigureAwait(false);
            result = await GetValueAsync2().ConfigureAwait(false);
            result = GetValue();
        }

        private static Task<object> GetValueAsync2()
        {
            return Task.FromResult(new object());
        }

        public static object GetValue()
        {
            return null;
        }
    }
}
