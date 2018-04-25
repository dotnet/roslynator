// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public abstract class DiagnosticDescriptorComparer : IComparer<DiagnosticDescriptor>, IEqualityComparer<DiagnosticDescriptor>, IComparer, IEqualityComparer
    {
        public static DiagnosticDescriptorComparer Id { get; } = new DiagnosticDescriptorIdOrdinalComparer();

        public abstract int Compare(DiagnosticDescriptor x, DiagnosticDescriptor y);

        public abstract int Compare(object x, object y);

        public abstract bool Equals(DiagnosticDescriptor x, DiagnosticDescriptor y);

        new public abstract bool Equals(object x, object y);

        public abstract int GetHashCode(DiagnosticDescriptor obj);

        public abstract int GetHashCode(object obj);

        private class DiagnosticDescriptorIdOrdinalComparer : DiagnosticDescriptorComparer
        {
            public override int Compare(DiagnosticDescriptor x, DiagnosticDescriptor y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                return string.Compare(x.Id, y.Id, StringComparison.Ordinal);
            }

            public override int Compare(object x, object y)
            {
                return Compare(x as DiagnosticDescriptor, y as DiagnosticDescriptor);
            }

            public override bool Equals(DiagnosticDescriptor x, DiagnosticDescriptor y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return string.Equals(x.Id, y.Id, StringComparison.Ordinal);
            }

            public override bool Equals(object x, object y)
            {
                return Equals(x as DiagnosticDescriptor, y as DiagnosticDescriptor);
            }

            public override int GetHashCode(DiagnosticDescriptor obj)
            {
                return StringComparer.Ordinal.GetHashCode(obj?.Id);
            }

            public override int GetHashCode(object obj)
            {
                return GetHashCode(obj as DiagnosticDescriptor);
            }
        }
    }
}
