// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class MergeLocalDeclarationWithReturnStatement
    {
        public static bool Foo()
        {
            bool condition = false;

            return condition;
        }

        public static bool Foo()
        {
            bool condition = false;

            bool condition2 = false;

            return condition;
        }
    }
}
