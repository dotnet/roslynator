// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Roslynator
{
    internal static class OneOrMany
    {
        public static OneOrMany<T> Create<T>(T value)
        {
            return new OneOrMany<T>(value);
        }

        public static OneOrMany<T> Create<T>(IEnumerable<T> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return Create(values.ToImmutableArray());
        }

        public static OneOrMany<T> Create<T>(ImmutableArray<T> values)
        {
            if (values.IsDefault)
                throw new ArgumentException("Immutable array is not initialized.", nameof(values));

            return new OneOrMany<T>(values);
        }
    }
}
