// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator.CodeFixes
{
    internal readonly struct MultipleFixesInfo : IEquatable<MultipleFixesInfo>
    {
        public MultipleFixesInfo(string diagnosticId, CodeFixProvider fixer, string equivalenceKey1, string equivalenceKey2)
        {
            DiagnosticId = diagnosticId;
            Fixer = fixer;
            EquivalenceKey1 = equivalenceKey1;
            EquivalenceKey2 = equivalenceKey2;
        }

        public string DiagnosticId { get; }

        public CodeFixProvider Fixer { get; }

        public string EquivalenceKey1 { get; }

        public string EquivalenceKey2 { get; }

        public override bool Equals(object obj)
        {
            return obj is MultipleFixesInfo other && Equals(other);
        }

        public bool Equals(MultipleFixesInfo other)
        {
            return DiagnosticId == other.DiagnosticId
                && Fixer == other.Fixer
                && EquivalenceKey1 == other.EquivalenceKey1
                && EquivalenceKey2 == other.EquivalenceKey2;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(DiagnosticId,
                Hash.Combine(Fixer,
                Hash.Combine(EquivalenceKey1,
                Hash.Create(EquivalenceKey2))));
        }

        public static bool operator ==(in MultipleFixesInfo info1, in MultipleFixesInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in MultipleFixesInfo info1, in MultipleFixesInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
