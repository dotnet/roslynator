// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ChangeForeachTypeAccordingToExpressionRefactoring
    {
        public void SomeMethod()
        {
            var items = new List<long>();

            foreach (string item in items)
            {







            }
        }
    }
}
