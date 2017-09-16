// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class FormatExpressionChainRefactoring
    {
        private class FormatExpressionChainOnMultipleLinesRefactoring
        {
            public void SomeMethod()
            {
                var value = new List<string>();

                var items = value.Where(f => f.Length > 0).Select(f => f[0]);
            }
        }

        private class FormatExpressionChainOnSingleLineRefactoring
        {
            public void SomeMethod()
            {
                var value = new List<string>();

                var items = value
                    .Where(f => f.Length > 0)
                    .Select(f => f[0]);
            }
        }
    }
}
