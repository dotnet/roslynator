// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    [DebuggerDisplay("{Flags}")]
    internal readonly struct TypeAnalysis : IEquatable<TypeAnalysis>
    {
        internal TypeAnalysis(ITypeSymbol symbol, TypeAnalysisFlags flags)
        {
            Symbol = symbol;
            Flags = flags;
        }

        public bool IsImplicit => Any(TypeAnalysisFlags.Implicit);

        public bool IsExplicit => Any(TypeAnalysisFlags.Explicit);

        public bool SupportsImplicit => Any(TypeAnalysisFlags.SupportsImplicit);

        public bool SupportsExplicit => Any(TypeAnalysisFlags.SupportsExplicit);

        public bool IsTypeObvious => Any(TypeAnalysisFlags.TypeObvious);

        public ITypeSymbol Symbol { get; }

        public TypeAnalysisFlags Flags { get; }

        public bool Any(TypeAnalysisFlags flags)
        {
            return (Flags & flags) != 0;
        }

        public override bool Equals(object obj)
        {
            return obj is TypeAnalysis other && Equals(other);
        }

        public bool Equals(TypeAnalysis other)
        {
            return Symbol == other.Symbol;
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode();
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
