// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public abstract class DiagnosticComparer : IComparer<Diagnostic>, IEqualityComparer<Diagnostic>, IComparer, IEqualityComparer
    {
        public static DiagnosticComparer Id { get; } = new DiagnosticIdOrdinalComparer();

        public static DiagnosticComparer SpanStart { get; } = new DiagnosticSpanStartComparer();

        public abstract int Compare(Diagnostic x, Diagnostic y);

        public abstract int Compare(object x, object y);

        public abstract bool Equals(Diagnostic x, Diagnostic y);

        new public abstract bool Equals(object x, object y);

        public abstract int GetHashCode(Diagnostic obj);

        public abstract int GetHashCode(object obj);

        private class DiagnosticIdOrdinalComparer : DiagnosticComparer
        {
            public override int Compare(Diagnostic x, Diagnostic y)
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
                return Compare(x as Diagnostic, y as Diagnostic);
            }

            public override bool Equals(Diagnostic x, Diagnostic y)
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
                return Equals(x as Diagnostic,y as Diagnostic);
            }

            public override int GetHashCode(Diagnostic obj)
            {
                return StringComparer.Ordinal.GetHashCode(obj?.Id);
            }

            public override int GetHashCode(object obj)
            {
                return GetHashCode(obj as Diagnostic);
            }
        }

        private class DiagnosticSpanStartComparer : DiagnosticComparer
        {
            public override int Compare(Diagnostic x, Diagnostic y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                return Comparer<int>.Default.Compare(x.Location.SourceSpan.Start, y.Location.SourceSpan.Start);
            }

            public override int Compare(object x, object y)
            {
                return Compare(x as Diagnostic, y as Diagnostic);
            }

            public override bool Equals(Diagnostic x, Diagnostic y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return x.Location.SourceSpan.Start == y.Location.SourceSpan.Start;
            }

            public override bool Equals(object x, object y)
            {
                return Equals(x as Diagnostic, y as Diagnostic);
            }

            public override int GetHashCode(Diagnostic obj)
            {
                return obj?.Location.SourceSpan.Start.GetHashCode() ?? 0;
            }

            public override int GetHashCode(object obj)
            {
                return GetHashCode(obj as Diagnostic);
            }
        }
    }
}
