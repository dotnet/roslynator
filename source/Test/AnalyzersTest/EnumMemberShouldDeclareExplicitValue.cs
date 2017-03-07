// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Test
{
    public static class EnumMemberShouldDeclareExplicitValue
    {
        [Flags]
        private enum FooWithFlags
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
            A = 1,
            B = 2,
            C,
            D = 4,
            E,
        }
    }
}
