// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

#pragma warning disable RCS1130

namespace Roslynator.CSharp
{
    /// <summary>
    /// Specifies a null check.
    /// </summary>
    [Flags]
    public enum NullCheckStyles
    {
        /// <summary>
        /// No null check specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// <code>x == null</code>
        /// </summary>
        EqualsToNull = 1,

        /// <summary>
        /// <code>x != null</code>
        /// </summary>
        NotEqualsToNull = 2,

        /// <summary>
        /// Expression that uses equality/inequality operator.
        /// </summary>
        ComparisonToNull = EqualsToNull | NotEqualsToNull,

        /// <summary>
        /// <code>x is null</code>
        /// </summary>
        IsNull = 4,

        /// <summary>
        /// <code>!(x is null)</code>
        /// </summary>
        NotIsNull = 8,

        /// <summary>
        /// Expression that uses pattern syntax.
        /// </summary>
        IsPattern = IsNull | NotIsNull,

        /// <summary>
        /// <code>!x.HasValue</code>
        /// </summary>
        NotHasValue = 16,

        /// <summary>
        /// Expression that checks whether an expression is null.
        /// </summary>
        CheckingNull = EqualsToNull | IsNull | NotHasValue,

        /// <summary>
        /// <code>x.HasValue</code>
        /// </summary>
        HasValue = 32,

        /// <summary>
        /// Expression that checks whether an expression is not null.
        /// </summary>
        CheckingNotNull = NotEqualsToNull | NotIsNull | HasValue,

        /// <summary>
        /// Expression that uses <see cref="Nullable{T}.HasValue"/> property.
        /// </summary>
        HasValueProperty = HasValue | NotHasValue,

        /// <summary>
        /// All null check styles.
        /// </summary>
        All = CheckingNull | CheckingNotNull,
    }
}
