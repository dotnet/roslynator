// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ExtractGenericTypeRefactoring
    {
        public IEnumerable<string> SomeMethod()
        {
            yield break;
        }
    }
}
