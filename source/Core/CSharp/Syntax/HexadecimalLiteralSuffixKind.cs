// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Syntax
{
    public enum HexadecimalLiteralSuffixKind
    {
        None,
        UInt32OrUInt64,
        Int64OrUInt64,
        UInt64,
        Unknown
    }
}