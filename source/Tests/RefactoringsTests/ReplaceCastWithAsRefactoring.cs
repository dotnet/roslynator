// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ReplaceCastWithAsRefactoring
    {
        public void Foo()
        {
            object value = GetValue();

            var entity = (Entity)value;
        }

        private object GetValue()
        {
            return null;
        }

        private class Entity
        {
        }
    }
}
