// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ReplaceStringFormatWithInterpolatedStringRefactoring
    {
        public void SomeMethod()
        {
            string name = "name";
            string value = "value";

            string s = string.Format("name: {0}, value: {1}", name, value);
        }
    }
}
