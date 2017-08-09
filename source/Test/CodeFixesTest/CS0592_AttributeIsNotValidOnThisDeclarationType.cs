// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.CodeFixes.Test
{
    internal static class CS0592_AttributeIsNotValidOnThisDeclarationType
    {
        [Flags]
        private class Foo
        {
        }

        [Obsolete, ParamArray]
        private class Foo2
        {
        }
    }
}
