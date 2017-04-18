// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.Utilities
{
    internal struct Deferred<T> : IEquatable<Deferred<T>>
    {
        public Deferred(T value)
        {
            Value = value;
            IsSet = true;
        }

        public T Value { get; }

        public bool IsSet { get; }

        public bool Equals(Deferred<T> other)
        {
            return object.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is Deferred<T>
                && object.Equals(Value, ((Deferred<T>)obj).Value);
        }

        public override int GetHashCode()
        {
            return (!EqualityComparer<T>.Default.Equals(Value, default(T)))
                ? Value.GetHashCode()
                : 0;
        }

        public static bool operator ==(Deferred<T> left, Deferred<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Deferred<T> left, Deferred<T> right)
        {
            return !left.Equals(right);
        }
    }
}
