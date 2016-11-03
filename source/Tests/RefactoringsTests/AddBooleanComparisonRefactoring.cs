// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class AddBooleanComparisonRefactoring
    {
        public void SomeMethod()
        {
            string value = GetValueOrDefault();

            if (value?.StartsWith("a"))
            {

            }

            if (!value?.StartsWith("a"))
            {

            }
        }

        private string GetValueOrDefault()
        {
            return null;
        }

        private bool GetValue()
        {
            return GetValueOrDefault()?.StartsWith("a");
        }

        private bool GetValue2() => GetValueOrDefault()?.StartsWith("a");
    }
}
