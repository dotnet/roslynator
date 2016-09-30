// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
namespace Pihrtsoft.CodeAnalysis.CSharp.Analyzers.Tests
{
    internal class AddConfigureAwait
    {
        public static async Task MethodName()
        {
            object result = await GetAsyncAsync();
            object result2 = await GetAsyncAsync();
        }

        private static Task<object> GetAsyncAsync()
        {
            return Task.FromResult(new object());
        }
    }
}
