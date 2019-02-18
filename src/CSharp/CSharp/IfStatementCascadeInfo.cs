// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace Roslynator.CSharp
{
    /// <summary>
    /// Summarizes information about <see cref="IfStatementCascade"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct IfStatementCascadeInfo : IEquatable<IfStatementCascadeInfo>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IfStatementCascadeInfo"/>.
        /// </summary>
        /// <param name="ifStatement"></param>
        public IfStatementCascadeInfo(IfStatementSyntax ifStatement)
        {
            int count = 0;
            IfStatementOrElseClause last = default;

            foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
            {
                count++;
                last = ifOrElse;
            }

            IfStatement = ifStatement;
            Count = count;
            Last = last;
        }

        /// <summary>
        /// Gets the topmost 'if' statement.
        /// </summary>
        public IfStatementSyntax IfStatement { get; }

        /// <summary>
        /// Gets a number of 'if' statements plus optional 'else' clause at the end of a cascade.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets a last 'if' or 'else' in a cascade.
        /// </summary>
        public IfStatementOrElseClause Last { get; }

        /// <summary>
        /// Determines whether the cascade ends with 'if' statement.
        /// </summary>
        public bool EndsWithIf => Last.IsIf;

        /// <summary>
        /// Determines whether the cascade ends with 'else' clause.
        /// </summary>
        public bool EndsWithElse => Last.IsElse;

        /// <summary>
        /// Determines whether the cascade consists of single 'if' statement.
        /// </summary>
        public bool IsSimpleIf => Count == 1;

        /// <summary>
        /// Determines whether the cascade consists of single if-else.
        /// </summary>
        public bool IsSimpleIfElse => Count == 2;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return (IfStatement != null) ? $"Count = {Count} {((EndsWithIf) ? $"EndsWithIf = {EndsWithIf}" : $"EndsWithElse = {EndsWithElse}")}" : "Uninitialized"; }
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
            return obj is IfStatementCascadeInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(IfStatementCascadeInfo other)
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

        public static bool operator ==(in IfStatementCascadeInfo info1, in IfStatementCascadeInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in IfStatementCascadeInfo info1, in IfStatementCascadeInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
