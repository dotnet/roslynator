// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.Spelling
{
    internal abstract class SpellingFixResultEqualityComparer : IEqualityComparer<SpellingFixResult>
    {
        public static SpellingFixResultEqualityComparer ValueAndReplacement { get; } = new ValueAndReplacementComparer();

        public static SpellingFixResultEqualityComparer ValueAndLineSpan { get; } = new ValueAndLineSpanComparer();

        public abstract bool Equals(SpellingFixResult x, SpellingFixResult y);

        public abstract int GetHashCode(SpellingFixResult obj);

        private class ValueAndReplacementComparer : SpellingFixResultEqualityComparer
        {
            public override bool Equals(SpellingFixResult x, SpellingFixResult y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return StringComparer.CurrentCulture.Equals(x.Value, y.Value)
                    && StringComparer.CurrentCulture.Equals(x.Replacement, y.Replacement);
            }

            public override int GetHashCode(SpellingFixResult obj)
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                return Hash.Combine(
                    StringComparer.CurrentCulture.GetHashCode(obj.Value),
                    StringComparer.CurrentCulture.GetHashCode(obj.Replacement));
            }
        }

        private class ValueAndLineSpanComparer : SpellingFixResultEqualityComparer
        {
            public override bool Equals(SpellingFixResult x, SpellingFixResult y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return StringComparer.CurrentCulture.Equals(x.Value, y.Value)
                    && StringComparer.CurrentCulture.Equals(x.LineSpan, y.LineSpan);
            }

            public override int GetHashCode(SpellingFixResult obj)
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                return Hash.Combine(
                    StringComparer.CurrentCulture.GetHashCode(obj.Value),
                    StringComparer.CurrentCulture.GetHashCode(obj.LineSpan));
            }
        }
    }
}
