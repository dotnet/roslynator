// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Roslynator.Text
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct IndentationChange : IEquatable<IndentationChange>
    {
        public IndentationChange(ImmutableArray<IndentationInfo> indentations, string replacement)
        {
            Indentations = indentations;
            Replacement = replacement;
        }

        public static IndentationChange Empty { get; } = new(ImmutableArray<IndentationInfo>.Empty, null);

        public ImmutableArray<IndentationInfo> Indentations { get; }

        public string Replacement { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return (Indentations.IsDefault)
                    ? "Uninitialized"
                    : $"Length = {Indentations.Length}  Replacement = \"{Replacement}\"";
            }
        }

        public override bool Equals(object obj)
        {
            return obj is IndentationChange other
                && Equals(other);
        }

        public bool Equals(IndentationChange other)
        {
            return Replacement == other.Replacement
                && Indentations.Equals(other.Indentations);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(
                Indentations.GetHashCode(),
                Hash.Create(Replacement));
        }

        public static bool operator ==(IndentationChange left, IndentationChange right) => left.Equals(right);

        public static bool operator !=(IndentationChange left, IndentationChange right) => !(left == right);
    }
}
