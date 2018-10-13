// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Roslynator.Documentation
{
    //XTODO: move to core
    internal readonly struct OneOrMany<T> : IReadOnlyList<T>, IEquatable<OneOrMany<T>>
    {
        private readonly State _state;
        private readonly T _value;
        private readonly ImmutableArray<T> _values;

        internal OneOrMany(T value)
        {
            _value = value;
            _values = default;
            _state = State.One;
        }

        internal OneOrMany(ImmutableArray<T> values)
        {
            Debug.Assert(!values.IsDefault, "Immutable array is not initialized.");

            _value = default;
            _values = values;
            _state = State.Many;
        }

        public T this[int index]
        {
            get
            {
                switch (_state)
                {
                    case State.One:
                        {
                            if (index == 0)
                                return _value;

                            break;
                        }
                    case State.Many:
                        {
                            return _values[index];
                        }
                }

                throw new ArgumentOutOfRangeException(nameof(index), index, "");
            }
        }

        public int Count
        {
            get
            {
                switch (_state)
                {
                    case State.One:
                        return 1;
                    case State.Many:
                        return _values.Length;
                    default:
                        return 0;
                }
            }
        }

        public bool IsDefault => _state == State.Default;

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (_state != State.Default)
                return new EnumeratorImpl(this);

            return Empty.Enumerator<T>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_state != State.Default)
                return new EnumeratorImpl(this);

            return Empty.Enumerator<T>();
        }

        public override bool Equals(object obj)
        {
            return obj is OneOrMany<T> other && Equals(other);
        }

        public bool Equals(OneOrMany<T> other)
        {
            if (_state == State.Default)
            {
                return other._state == State.Default;
            }
            else if (_state == State.One)
            {
                return other._state == State.One
                    && EqualityComparer<T>.Default.Equals(_value, other._value);
            }
            else if (_state == State.Many)
            {
                return other._state == State.Many
                    && _values.Equals(other._values);
            }

            throw new InvalidOperationException();
        }

        public override int GetHashCode()
        {
            int hashCode = Hash.Create((int)_state);

            if (_state == State.One)
                return Hash.Combine(EqualityComparer<T>.Default.GetHashCode(_value), hashCode);

            if (_state == State.Many)
                return Hash.Combine(Hash.CombineValues(_values), hashCode);

            return hashCode;
        }

        public static bool operator ==(in OneOrMany<T> oneOrMany1, in OneOrMany<T> oneOrMany2)
        {
            return oneOrMany1.Equals(oneOrMany2);
        }

        public static bool operator !=(in OneOrMany<T> oneOrMany1, in OneOrMany<T> oneOrMany2)
        {
            return !(oneOrMany1 == oneOrMany2);
        }

        public struct Enumerator
        {
            private readonly OneOrMany<T> _oneOrMany;
            private int _index;

            internal Enumerator(in OneOrMany<T> oneOrMany)
            {
                _oneOrMany = oneOrMany;
                _index = -1;
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _oneOrMany.Count;
            }

            public T Current
            {
                get { return _oneOrMany[_index]; }
            }

            public void Reset()
            {
                _index = -1;
            }

            public override bool Equals(object obj) => throw new NotSupportedException();

            public override int GetHashCode() => throw new NotSupportedException();
        }

        private class EnumeratorImpl : IEnumerator<T>
        {
            private Enumerator _en;

            internal EnumeratorImpl(in OneOrMany<T> oneOrMany)
            {
                _en = new Enumerator(oneOrMany);
            }

            public T Current => _en.Current;

            object IEnumerator.Current => _en.Current;

            public bool MoveNext() => _en.MoveNext();

            public void Reset() => _en.Reset();

            public void Dispose()
            {
            }
        }

        private enum State
        {
            Default,
            One,
            Many,
        }
    }
}
