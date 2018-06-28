// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp
{
    public readonly partial struct ExpressionChain
    {
        /// <summary>
        /// Enables to enumerate expressions of <see cref="ExpressionChain"/> in a reversed order.
        /// </summary>
        public readonly struct Reversed : IEquatable<Reversed>, IEnumerable<ExpressionSyntax>
        {
            private readonly ExpressionChain _chain;

            public Reversed(in ExpressionChain chain)
            {
                _chain = chain;
            }

            internal bool IsStringConcatenation(
                SemanticModel semanticModel,
                CancellationToken cancellationToken = default(CancellationToken))
            {
                if (!_chain.BinaryExpression.IsKind(SyntaxKind.AddExpression))
                    return false;

                Enumerator en = GetEnumerator();

                if (!en.MoveNext())
                    return false;

                var binaryExpression = (BinaryExpressionSyntax)en.Current.Parent;

                if (!en.MoveNext())
                    return false;

                while (true)
                {
                    if (!CSharpUtility.IsStringConcatenation(binaryExpression, semanticModel, cancellationToken))
                        return false;

                    ExpressionSyntax prev = en.Current;

                    if (en.MoveNext())
                    {
                        binaryExpression = (BinaryExpressionSyntax)prev.Parent;
                    }
                    else
                    {
                        break;
                    }
                }

                return true;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(_chain);
            }

            IEnumerator<ExpressionSyntax> IEnumerable<ExpressionSyntax>.GetEnumerator()
            {
                if (_chain.BinaryExpression == null)
                    return Empty.Enumerator<ExpressionSyntax>();

                return new EnumeratorImpl(_chain);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                if (_chain.BinaryExpression == null)
                    return Empty.Enumerator<ExpressionSyntax>();

                return new EnumeratorImpl(_chain);
            }

            public override string ToString()
            {
                return _chain.ToString();
            }

            public override bool Equals(object obj)
            {
                return obj is Reversed other && Equals(other);
            }

            public bool Equals(Reversed other)
            {
                return _chain.Equals(other._chain);
            }

            public override int GetHashCode()
            {
                return _chain.GetHashCode();
            }

            public static bool operator ==(in Reversed reversed1, in Reversed reversed2)
            {
                return reversed1.Equals(reversed2);
            }

            public static bool operator !=(in Reversed reversed1, in Reversed reversed2)
            {
                return !(reversed1 == reversed2);
            }

            [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
            [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
            [SuppressMessage("Usage", "CA2231:Overload operator equals on overriding value type Equals", Justification = "<Pending>")]
            public struct Enumerator
            {
                private ExpressionChain _chain;
                private ExpressionSyntax _current;
                private State _state;

                internal Enumerator(in ExpressionChain chain)
                {
                    _chain = chain;
                    _current = null;
                    _state = State.Start;
                }

                public bool MoveNext()
                {
                    switch (_state)
                    {
                        case State.Start:
                            {
                                if (_chain.BinaryExpression == null)
                                    return false;

                                if (_chain.Span == null)
                                {
                                    _current = _chain.BinaryExpression.Right;
                                    _state = State.Right;
                                    return true;
                                }

                                ExpressionSyntax right = _chain.BinaryExpression.Right;

                                TextSpan span = _chain.Span.Value;

                                if (IsInSpan(span, right.Span))
                                {
                                    _current = right;
                                    _state = State.Right;
                                    return true;
                                }

                                BinaryExpressionSyntax binaryExpression = _chain.BinaryExpression;

                                ExpressionSyntax left = null;

                                while (true)
                                {
                                    left = binaryExpression.Left;

                                    if (left.RawKind == binaryExpression.RawKind)
                                    {
                                        binaryExpression = (BinaryExpressionSyntax)left;
                                        right = binaryExpression.Right;

                                        if (IsInSpan(span, right.Span))
                                        {
                                            _current = right;
                                            _state = State.Right;
                                            return true;
                                        }
                                    }
                                    else if (IsInSpan(span, left.Span))
                                    {
                                        _current = left;
                                        _state = State.Left;
                                        return true;
                                    }
                                    else
                                    {
                                        _state = State.Left;
                                        return false;
                                    }
                                }
                            }
                        case State.Right:
                            {
                                var binaryExpression = (BinaryExpressionSyntax)_current.Parent;

                                ExpressionSyntax left = binaryExpression.Left;

                                if (_chain.Span == null)
                                {
                                    if (left.RawKind == binaryExpression.RawKind)
                                    {
                                        binaryExpression = (BinaryExpressionSyntax)left;

                                        _current = binaryExpression.Right;
                                        _state = State.Right;
                                    }
                                    else
                                    {
                                        _current = left;
                                        _state = State.Left;
                                    }

                                    return true;
                                }
                                else
                                {
                                    TextSpan span = _chain.Span.Value;

                                    if (left.RawKind == binaryExpression.RawKind)
                                    {
                                        binaryExpression = (BinaryExpressionSyntax)left;

                                        ExpressionSyntax right = binaryExpression.Right;

                                        if (IsInSpan(span, right.Span))
                                        {
                                            _current = right;
                                            _state = State.Right;
                                            return true;
                                        }
                                    }

                                    if (IsInSpan(span, left.Span))
                                    {
                                        _current = left;
                                        _state = State.Left;
                                        return true;
                                    }
                                    else
                                    {
                                        _current = null;
                                        _state = State.Left;
                                        return false;
                                    }
                                }
                            }
                        case State.Left:
                            {
                                return false;
                            }
                        default:
                            {
                                throw new InvalidOperationException();
                            }
                    }
                }

                public ExpressionSyntax Current
                {
                    get { return _current ?? throw new InvalidOperationException(); }
                }

                public void Reset()
                {
                    _current = null;
                    _state = State.Start;
                }

                public override bool Equals(object obj) => throw new NotSupportedException();

                public override int GetHashCode() => throw new NotSupportedException();

                private enum State
                {
                    Start = 0,
                    Left = 1,
                    Right = 2,
                }
            }

            private class EnumeratorImpl : IEnumerator<ExpressionSyntax>
            {
                private Enumerator _en;

                internal EnumeratorImpl(in ExpressionChain chain)
                {
                    _en = new Enumerator(chain);
                }

                public ExpressionSyntax Current => _en.Current;

                object IEnumerator.Current => _en.Current;

                public bool MoveNext() => _en.MoveNext();

                public void Reset() => _en.Reset();

                public void Dispose()
                {
                }
            }
        }
    }
}
