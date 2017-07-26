// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Syntax
{
    internal enum NullCheckKind
    {
        None = 0,
        EqualsToNull = 1,
        NotEqualsToNull = 2,
        HasValue = 3,
        NotHasValue = 4
    }
}
