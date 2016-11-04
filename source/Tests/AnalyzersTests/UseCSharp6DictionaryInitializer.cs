// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal class UseCSharp6DictionaryInitializer
    {
        public static void GetValue()
        {
            var dic = new Dictionary<int, string>() { { 0, "0" } };

            dic = new Dictionary<int, string>() { { 0, "0" }, { 0, "1" } };

            dic = new Dictionary<int, string>() { [0] = null };

            var items = new List<string>() { { null } };
        }
    }
}
