// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

#pragma warning disable RCS1013, RCS1127

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class SimplifyNullableOfT
    {
        public static void Foo(Nullable<int> x, string s)
        {
            x = default(Nullable<int>);
            x = default(System.Nullable<int>);
            x = default(global::System.Nullable<int>);

            x = default(Nullable<Int32>);
            x = default(System.Nullable<Int32>);
            x = default(global::System.Nullable<Int32>);

            s = nameof(List<Nullable<int>>);

            // n

            s = nameof(Nullable<int>);
            s = nameof(System.Nullable<int>);
            s = nameof(global::System.Nullable<int>);

            s = nameof(Nullable<int>.Value);
            s = nameof(System.Nullable<int>.Value);
            s = nameof(global::System.Nullable<int>.Value);
        }

        /// <summary>
        /// <see cref="Nullable{T}"/>
        /// <see cref="System.Nullable{T}"/>
        /// <see cref="global::System.Nullable{T}"/>
        /// <see cref="Nullable{T}.HasValue"/>
        /// <see cref="System.Nullable{T}.HasValue"/>
        /// <see cref="global::System.Nullable{T}.HasValue"/>
        /// </summary>
        public static void Foo()
        {
        }
    }
}
