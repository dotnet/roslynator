// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ExpandExpressionBodyRefactoring
    {
        public void GetValue() => GetValue2();

        public string GetValue2() => null;

        public string GetValue3() => throw new Exception();
    }
}
