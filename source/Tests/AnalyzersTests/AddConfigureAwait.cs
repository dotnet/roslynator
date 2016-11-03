// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal class AddConfigureAwait
    {
        public static async Task GetValueAsync()
        {
            object result = await GetValueAsync2();
            object result2 = await GetValueAsync2();
        }

        private static Task<object> GetValueAsync2()
        {
            return Task.FromResult(new object());
        }
    }
}
