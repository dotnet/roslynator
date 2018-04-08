// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    /// <summary>
    /// Enables to enumerate if statement cascade.
    /// </summary>
    public readonly struct IfStatementCascade : IEquatable<IfStatementCascade>, IEnumerable<IfStatementOrElseClause>
    {
        internal IfStatementCascade(IfStatementSyntax ifStatement)
        {
            IfStatement = ifStatement;
        }

        /// <summary>
        /// The if statement.
        /// </summary>
        public IfStatementSyntax IfStatement { get; }

        /// <summary>
        /// Gets the enumerator for the if-else cascade.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(IfStatement);
        }

        IEnumerator<IfStatementOrElseClause> IEnumerable<IfStatementOrElseClause>.GetEnumerator()
        {
            if (IfStatement != null)
                return new EnumeratorImpl(IfStatement);

            return Empty.Enumerator<IfStatementOrElseClause>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (IfStatement != null)
                return new EnumeratorImpl(IfStatement);

            return Empty.Enumerator<IfStatementOrElseClause>();
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return IfStatement?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is IfStatementCascade other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(IfStatementCascade other)
        {
            return EqualityComparer<IfStatementSyntax>.Default.Equals(IfStatement, other.IfStatement);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<IfStatementSyntax>.Default.GetHashCode(IfStatement);
        }

        public static bool operator ==(IfStatementCascade info1, IfStatementCascade info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(IfStatementCascade info1, IfStatementCascade info2)
        {
            return !(info1 == info2);
        }

#pragma warning disable CS1591
        public struct Enumerator
        {
            private IfStatementOrElseClause _ifOrElse;
            private int _count;

            internal Enumerator(IfStatementSyntax ifStatement)
            {
                _ifOrElse = ifStatement;
                _count = -1;
            }

            public bool MoveNext()
            {
                if (_count == -1)
                {
                    if (_ifOrElse != default(IfStatementOrElseClause))
                    {
                        _count++;
                        return true;
                    }
                }
                else if (_ifOrElse.IsIf)
                {
                    ElseClauseSyntax elseClause = _ifOrElse.AsIf().Else;

                    if (elseClause != null)
                    {
                        if (elseClause.Statement is IfStatementSyntax nextIf)
                        {
                            _ifOrElse = nextIf;
                        }
                        else
                        {
                            _ifOrElse = elseClause;
                        }

                        _count++;
                        return true;
                    }
                }

                return false;
            }

            public IfStatementOrElseClause Current
            {
                get { return (_count >= 0) ? _ifOrElse : throw new InvalidOperationException(); }
            }

            public void Reset()
            {
                int count = _count;

                if (count >= 0)
                {
                    IfStatementSyntax ifStatement = null;

                    if (_ifOrElse.IsElse)
                    {
                        ifStatement = (IfStatementSyntax)_ifOrElse.Parent;
                    }
                    else
                    {
                        ifStatement = _ifOrElse.AsIf();
                    }

                    count--;

                    while (count >= 0)
                    {
                        ifStatement = (IfStatementSyntax)ifStatement.Parent.Parent;
                        count--;
                    }

                    _ifOrElse = ifStatement;
                }
            }

            public override bool Equals(object obj)
            {
                throw new NotSupportedException();
            }

            public override int GetHashCode()
            {
                throw new NotSupportedException();
            }
        }

        private class EnumeratorImpl : IEnumerator<IfStatementOrElseClause>
        {
            private Enumerator _en;

            internal EnumeratorImpl(IfStatementSyntax ifStatement)
            {
                _en = new Enumerator(ifStatement);
            }

            public IfStatementOrElseClause Current
            {
                get { return _en.Current; }
            }

            object IEnumerator.Current
            {
                get { return _en.Current; }
            }

            public bool MoveNext()
            {
                return _en.MoveNext();
            }

            public void Reset()
            {
                _en.Reset();
            }

            public void Dispose()
            {
            }
        }
#pragma warning disable CS1591
    }
}
