// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class FormatEachEnumMemberOnSeparateLine
    {
        [Flags]
        private enum FooWithFlags
        {
            A,
            B,
            C,
            D,
            E,
            F
        }

        [Flags]
        private enum FooWithFlags2
        {
            A = 1,
            B,
            C = 4,
            D,
            E = 16,
            F
        }

        private enum FooWithoutFlags
        {
            _,
            A,
            B,
            C,
            D,
            E,
        }

        private enum FooWithoutFlags2
        {
            _,
            A = 1,
            B = 2,
            C,
            D = 4,
            E,
        }
    }
}
