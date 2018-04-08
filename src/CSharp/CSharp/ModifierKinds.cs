// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp
{
    /// <summary>
    /// Specifies C# modifier.
    /// </summary>
    [Flags]
    public enum ModifierKinds
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
        Public = 2,

        /// <summary>
        /// A "private" modifier.
        /// </summary>
        Private = 4,

        /// <summary>
        /// A "protected" modifier.
        /// </summary>
        Protected = 8,

        /// <summary>
        /// An "internal" modifier.
        /// </summary>
        Internal = 16,

        /// <summary>
        /// An accessibility modifier.
        /// </summary>
        Accessibility = Public | Private | Protected | Internal,

        /// <summary>
        /// A "const" modifier.
        /// </summary>
        Const = 32,

        /// <summary>
        /// A "static" modifier.
        /// </summary>
        Static = 64,

        /// <summary>
        /// A "virtual" modifier.
        /// </summary>
        Virtual = 128,

        /// <summary>
        /// A "sealed" modifier.
        /// </summary>
        Sealed = 256,

        /// <summary>
        /// An "override" modifier.
        /// </summary>
        Override = 512,

        /// <summary>
        /// An "abstract" modifier.
        /// </summary>
        Abstract = 1024,

        /// <summary>
        /// "abstract", "virtual" or "override" modifier.
        /// </summary>
        AbstractVirtualOverride = Abstract | Virtual | Override,

        /// <summary>
        /// A "readonly" modifier.
        /// </summary>
        ReadOnly = 2048,

        /// <summary>
        /// An "extern" modifier.
        /// </summary>
        Extern = 4096,

        /// <summary>
        /// A "unsafe" modifier.
        /// </summary>
        Unsafe = 8192,

        /// <summary>
        /// A "volatile" modifier.
        /// </summary>
        Volatile = 16384,

        /// <summary>
        /// An "async" modifier.
        /// </summary>
        Async = 32768,

        /// <summary>
        /// A "partial" modifier.
        /// </summary>
        Partial = 65536,

        /// <summary>
        /// A "ref" modifier.
        /// </summary>
        Ref = 131072,

        /// <summary>
        /// An "out" modifier.
        /// </summary>
        Out = 262144,

        /// <summary>
        /// An "in" modifier.
        /// </summary>
        In = 524288,

        /// <summary>
        /// A "params" modifier.
        /// </summary>
        Params = 1048576,
    }
}
