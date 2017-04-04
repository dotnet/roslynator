// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Roslynator.CSharp.Analyzers.Test
{
    public static class RemoveInapplicableModifier
    {
        private interface InterfaceName123
        {
            public string Value { get; private set; }
        }

        public static void MethodName()
        {
            private static async Task<bool> LocalFunction()
            {
                bool f = await Task.FromResult(false);
                return f;
            }
        }

        public static event EventHandler EventName
        {
            private add { }
            private remove { }
        }
    }
}