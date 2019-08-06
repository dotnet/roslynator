// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Syntax
{
    public enum IsKindExpressionStyle
    {
        None = 0,
        IsKind = 1,
        IsKindConditional = 2,
        Kind = 3,
        KindConditional = 4,
        NotIsKind = 5,
        NotIsKindConditional = 6,
        NotKind = 7,
        NotKindConditional = 8,
    }
}
