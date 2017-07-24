// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp
{
    [Flags]
    internal enum StringExpressionKind
    {
        None = 0,
        Literal = 1,
        InterpolatedString = 2,
        Expression = 4
    }
}
