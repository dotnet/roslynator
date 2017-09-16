// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class CheckExpressionForNullRefactoring
    {
        public static void Foo()
        {
            Entity x = GetValueOrDefault();

            x = GetValueOrDefault();

            x = new Entity();

            int i = GetValue();

            if (true)
                x = GetValueOrDefault();

            i = GetValue();

            int j = GetValue();
        }

        private static Entity GetValueOrDefault()
        {
            return null;
        }

        private static int GetValue()
        {
            return 0;
        }

        private class Entity
        {
        }
    }
}
