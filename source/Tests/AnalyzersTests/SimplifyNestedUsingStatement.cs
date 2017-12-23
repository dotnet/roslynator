// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class SimplifyNestedUsingStatement
    {
        private static void Foo()
        {
            using (var fs = new FileStream("path", FileMode.OpenOrCreate))
            {
                using (var sr = new StreamReader(fs))
                {
                }
            }
        }
    }
}
