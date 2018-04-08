// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

#pragma warning disable RCS1161

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class FormatEachEnumMemberOnSeparateLine
    {
        private enum Foo
        {
            A, B, C, D,
        }

        private enum Foo2
        {
            A = 0, B = 1, C = 2, D = 3,
        }

        private enum Foo3
        {
            A, B, C, D
        }

        //n

        private enum Foo4
        {
            A
        }

        private enum Foo5
        {
            A,
            B,
            C,
            D,
        }
    }
}
