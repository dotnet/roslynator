// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class DeclareEnumMemberWithZeroValue
    {
        [Flags]
        private enum Foo
        {
            Bar = 1
        }

        private enum Foo2
        {
            Bar = 1
        }

        [Flags]
        private enum Foo3
        {
            None = 0,
            Bar = 1
        }
    }
}
