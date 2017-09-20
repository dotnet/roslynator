// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ChangeVarToExplicitTypeRefactoring
    {
        public void SomeMethod()
        {
            var value = GetValue();

            string xx = GetObject();
        }

        private string GetValue()
        {
            return string.Empty;
        }

        private object GetObject()
        {
            return null;
        }

        public async void GetValueAsync()
        {
            var x = Task.FromResult(false);

            string xx = Task.FromResult(false);
        }
    }
}
