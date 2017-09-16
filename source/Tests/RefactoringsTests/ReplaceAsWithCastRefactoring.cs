// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ReplaceAsWithCastRefactoring
    {
        public void Foo()
        {
            object value = GetValue();

            var entity = value as Entity;

            if (entity != null)
            {
            }
        }

        private object GetValue()
        {
            return null;
        }
    }
}
