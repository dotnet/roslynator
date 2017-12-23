// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

#pragma warning disable RCS1130

namespace Roslynator.CSharp.Syntax
{
    [Flags]
    public enum NullCheckKind
    {
        None = 0,
        EqualsToNull = 1,
        NotEqualsToNull = 2,
        ComparisonToNull = EqualsToNull | NotEqualsToNull,
        HasValue = 4,
        IsNotNull = NotEqualsToNull | HasValue,
        NotHasValue = 8,
        IsNull = EqualsToNull | NotHasValue,
        HasValueProperty = HasValue | NotHasValue,
        All = EqualsToNull | NotEqualsToNull | HasValue | NotHasValue
    }
}
