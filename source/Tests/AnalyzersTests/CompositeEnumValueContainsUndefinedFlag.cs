// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class CompositeEnumValueContainsUndefinedFlag
    {
        [Flags]
        private enum Foo
        {
            None = 0,
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            ABD = 11,
            ABCD = 15,
            X = 17
        }
    }
}
