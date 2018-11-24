// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Roslynator
{
    [DebuggerDisplay("{Flags}")]
    internal readonly struct TypeAnalysis : IEquatable<TypeAnalysis>
    {
        internal TypeAnalysis(TypeAnalysisFlags flags)
        {
            Flags = flags;
        }

        public bool IsImplicit => Any(TypeAnalysisFlags.Implicit);

        public bool IsExplicit => Any(TypeAnalysisFlags.Explicit);

        public bool SupportsImplicit => Any(TypeAnalysisFlags.SupportsImplicit);

        public bool SupportsExplicit => Any(TypeAnalysisFlags.SupportsExplicit);

        public bool IsTypeObvious => Any(TypeAnalysisFlags.TypeObvious);

        internal TypeAnalysisFlags Flags { get; }

        public bool Any(TypeAnalysisFlags flags)
        {
            return (Flags & flags) != 0;
        }

        public bool All(TypeAnalysisFlags flags)
        {
            return (Flags & flags) != flags;
        }

        public override bool Equals(object obj)
        {
            return obj is TypeAnalysis other && Equals(other);
        }

        public bool Equals(TypeAnalysis other)
        {
            return Flags == other.Flags;
        }

        public override int GetHashCode()
        {
            return Flags.GetHashCode();
        }

        public static implicit operator TypeAnalysis(TypeAnalysisFlags value)
        {
            return new TypeAnalysis(value);
        }

        public static implicit operator TypeAnalysisFlags(in TypeAnalysis value)
        {
            return value.Flags;
        }

        public static bool operator ==(in TypeAnalysis analysis1, in TypeAnalysis analysis2)
        {
            return analysis1.Equals(analysis2);
        }

        public static bool operator !=(in TypeAnalysis analysis1, in TypeAnalysis analysis2)
        {
            return !(analysis1 == analysis2);
        }
    }
}
