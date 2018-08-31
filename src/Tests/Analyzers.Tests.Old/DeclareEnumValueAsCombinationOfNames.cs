// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

#pragma warning disable RCS1135, RCS1157

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class DeclareEnumValueAsCombinationOfNames
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

        [Flags]
        private enum Foo2
        {
            None = 0,
            A = 1,
            B = 2,
            AB = 3,
            C = 4,
            D = 8,
            ABD = 11,
            ABCD = 15,
        }

        [Flags]
        private enum Foo3
        {
            None = 0,
            AB = 3,
            C = 4,
            D = 8,
            ABD = 11,
            ABCD = 15,
        }

        [Flags]
        private enum Foo4
        {
            None = 0,
            A = 1,
            B = 2,
            AB = 3,
            C = 4,
            D = 8,
            ABD = 3 | D,
            ABCD = 1 | 2 | C | D,
        }

        [Flags]
        private enum Foosbyte : sbyte
        {
            A = 1,
            B = 2,
            AB = 3
        }

        [Flags]
        private enum Foobyte : byte
        {
            A = 1,
            B = 2,
            AB = 3
        }

        [Flags]
        private enum Fooshort : short
        {
            A = 1,
            B = 2,
            AB = 3
        }

        [Flags]
        private enum Fooushort : ushort
        {
            A = 1,
            B = 2,
            AB = 3
        }

        [Flags]
        private enum Foouint : uint
        {
            A = 1,
            B = 2,
            AB = 3
        }

        [Flags]
        private enum Foolong : long
        {
            A = 1,
            B = 2,
            AB = 3
        }

        [Flags]
        private enum Fooulong : ulong
        {
            A = 1,
            B = 2,
            AB = 3
        }

        // n

        [Flags]
        private enum Foo_
        {
            B = 2,
            AB = 3
        }
    }
}
