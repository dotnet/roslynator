// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.CodeFixes.Test
{
    internal static class AddTypeArgument
    {
        private class Base<T>
        {
        }

        private class Foo : Base
        {
        }

        private class Base<T, T2>
        {
        }

        private class Foo2 : Base
        {
        }
    }
}
