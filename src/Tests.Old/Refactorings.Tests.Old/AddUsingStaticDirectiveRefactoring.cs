// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class AddUsingStaticDirectiveRefactoring
    {
        public void Do()
        {
            int max = Math.Max(1, 2);

            max = System.Math.Max(1, 2);
            max = global::System.Math.Max(1, 2);
        }
    }
}
