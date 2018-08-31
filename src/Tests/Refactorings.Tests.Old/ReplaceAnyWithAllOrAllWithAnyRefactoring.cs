// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ReplaceAnyWithAllOrAllWithAnyRefactoring
    {
        public void SomeMethod()
        {
            var items = new List<string>();

            if (items.Any(f => f.Contains("a")))
            {
            }

            if (items.Any((f) => f.Contains("a")))
            {
            }
        }
    }
}
