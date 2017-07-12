// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class ReplaceCountMethodWithAnyMethod
    {
        private static void Foo()
        {
            IEnumerable<int> sequence = Enumerable.Range(0, 1);

            if (sequence.Count() == 0)
            {
            }

            if (sequence.Count() > 0)
            {
            }

            if (sequence.Count() >= 1)
            {
            }

            if (0 == sequence.Count())
            {
            }

            if (0 < sequence.Count())
            {
            }

            if (1 <= sequence.Count())
            {
            }
        }
    }
}
