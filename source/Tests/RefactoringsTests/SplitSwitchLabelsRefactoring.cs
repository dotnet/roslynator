// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class SplitSwitchLabelsRefactoring
    {
        private static void Foo()
        {
            StringComparison comparison = StringComparison.CurrentCulture;

            switch (comparison)
            {
                case StringComparison.CurrentCulture:
                case StringComparison.CurrentCultureIgnoreCase:
                case StringComparison.Ordinal:
                case StringComparison.OrdinalIgnoreCase:
                    break;
            }
        }
    }
}
