// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class RemoveParameterNameFromArgumentRefactoring
    {
        public void SomeMethod(string value, params string[] additionalValues)
        {
            string s = null;
            var values = new string[0];

            SomeMethod(value: s, additionalValues: values);










        }
    }
}
