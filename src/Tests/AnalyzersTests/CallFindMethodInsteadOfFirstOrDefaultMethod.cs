// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class CallFindMethodInsteadOfFirstOrDefaultMethod
    {
        public static void Foo()
        {
            var items = new List<string>();

            string x = items.FirstOrDefault(f => f == null);

            var arr = new string[] { null };

            string y = arr.FirstOrDefault(f => f == null);
        }
    }
}
