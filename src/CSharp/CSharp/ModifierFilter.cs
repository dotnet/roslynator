// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp
{
    /// <summary>
    /// Specifies C# modifier.
    /// </summary>
    [Flags]
    public enum ModifierFilter
    {
        /// <summary>
        /// None modifier.
        /// </summary>
        None = 0,

        /// <summary>
        /// A "new" modifier.
        /// </summary>
        New = 1,

        /// <summary>
        /// A "public" modifier.
        /// </summary>
        Public = 1 << 1,

        /// <summary>
        /// A "private" modifier.
        /// </summary>
        Private = 1 << 2,

        /// <summary>
        /// A "protected" modifier.
        /// </summary>
        Protected = 1 << 3,

        /// <summary>
        /// An "internal" modifier.
        /// </summary>
        Internal = 1 << 4,

        /// <summary>
        /// An accessibility modifier.
        /// </summary>
        Accessibility = Public | Private | Protected | Internal,

        /// <summary>
        /// A "const" modifier.
        /// </summary>
        Const = 1 << 5,

        /// <summary>
        /// A "static" modifier.
        /// </summary>
        Static = 1 << 6,

        /// <summary>
        /// A "virtual" modifier.
        /// </summary>
        Virtual = 1 << 7,

        /// <summary>
        /// A "sealed" modifier.
        /// </summary>
        Sealed = 1 << 8,

        /// <summary>
        /// An "override" modifier.
        /// </summary>
        Override = 1 << 9,

        /// <summary>
        /// An "abstract" modifier.
        /// </summary>
        Abstract = 1 << 10,

        /// <summary>
        /// "abstract", "virtual" or "override" modifier.
        /// </summary>
        AbstractVirtualOverride = Abstract | Virtual | Override,

        /// <summary>
        /// A "readonly" modifier.
        /// </summary>
        ReadOnly = 1 << 11,

        /// <summary>
        /// An "extern" modifier.
        /// </summary>
        Extern = 1 << 12,

        /// <summary>
        /// A "unsafe" modifier.
        /// </summary>
        Unsafe = 1 << 13,

        /// <summary>
        /// A "volatile" modifier.
        /// </summary>
        Volatile = 1 << 14,

        /// <summary>
        /// An "async" modifier.
        /// </summary>
        Async = 1 << 15,

        /// <summary>
        /// A "partial" modifier.
        /// </summary>
        Partial = 1 << 16,

        /// <summary>
        /// A "ref" modifier.
        /// </summary>
        Ref = 1 << 17,

        /// <summary>
        /// An "out" modifier.
        /// </summary>
        Out = 1 << 18,

        /// <summary>
        /// An "in" modifier.
        /// </summary>
        In = 1 << 19,

        /// <summary>
        /// A "params" modifier.
        /// </summary>
        Params = 1 << 20,

        /// <summary>
        /// A "this" modifier.
        /// </summary>
        This = 1 << 21,
    }
}
