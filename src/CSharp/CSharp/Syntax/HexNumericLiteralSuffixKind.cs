// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Syntax
{
    internal enum HexNumericLiteralSuffixKind
    {
        None = 0,
        UIntOrULong = 1,
        LongOrULong = 2,
        ULong = 3,
        Unknown = 4,
    }
}