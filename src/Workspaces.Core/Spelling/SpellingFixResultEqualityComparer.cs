// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.Spelling
{
    internal abstract class SpellingFixResultEqualityComparer : IEqualityComparer<SpellingFixResult>
    {
        public static SpellingFixResultEqualityComparer OldValueAndNewValue { get; } = new OldValueAndNewValueComparer();

        public static SpellingFixResultEqualityComparer OldIdentifierAndNewIdentifier { get; } = new OldIdentifierAndNewIdentifierComparer();

        public abstract bool Equals(SpellingFixResult x, SpellingFixResult y);

        public abstract int GetHashCode(SpellingFixResult obj);

        private class OldValueAndNewValueComparer : SpellingFixResultEqualityComparer
        {
            public override bool Equals(SpellingFixResult x, SpellingFixResult y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return StringComparer.CurrentCulture.Equals(x.OldValue, y.OldValue)
                    && StringComparer.CurrentCulture.Equals(x.NewValue, y.NewValue);
            }

            public override int GetHashCode(SpellingFixResult obj)
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                return Hash.Combine(
                    StringComparer.CurrentCulture.GetHashCode(obj.OldValue),
                    StringComparer.CurrentCulture.GetHashCode(obj.NewValue));
            }
        }

        private class OldIdentifierAndNewIdentifierComparer : SpellingFixResultEqualityComparer
        {
            public override bool Equals(SpellingFixResult x, SpellingFixResult y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return StringComparer.CurrentCulture.Equals(x.OldIdentifier, y.OldIdentifier)
                    && StringComparer.CurrentCulture.Equals(x.NewIdentifier, y.NewIdentifier);
            }

            public override int GetHashCode(SpellingFixResult obj)
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                return Hash.Combine(
                    StringComparer.CurrentCulture.GetHashCode(obj.OldIdentifier),
                    StringComparer.CurrentCulture.GetHashCode(obj.NewIdentifier));
            }
        }
    }
}
