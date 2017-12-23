// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class AccessibilityExtensions
    {
        public static bool Is(this Accessibility accessibility, Accessibility accessibility1, Accessibility accessibility2)
        {
            return accessibility == accessibility1
                || accessibility == accessibility2;
        }

        public static bool Is(this Accessibility accessibility, Accessibility accessibility1, Accessibility accessibility2, Accessibility accessibility3)
        {
            return accessibility == accessibility1
                || accessibility == accessibility2
                || accessibility == accessibility3;
        }

        public static bool Is(this Accessibility accessibility, Accessibility accessibility1, Accessibility accessibility2, Accessibility accessibility3, Accessibility accessibility4)
        {
            return accessibility == accessibility1
                || accessibility == accessibility2
                || accessibility == accessibility3
                || accessibility == accessibility4;
        }

        public static bool Is(this Accessibility accessibility, Accessibility accessibility1, Accessibility accessibility2, Accessibility accessibility3, Accessibility accessibility4, Accessibility accessibility5)
        {
            return accessibility == accessibility1
                || accessibility == accessibility2
                || accessibility == accessibility3
                || accessibility == accessibility4
                || accessibility == accessibility5;
        }

        public static bool Is(this Accessibility accessibility, Accessibility accessibility1, Accessibility accessibility2, Accessibility accessibility3, Accessibility accessibility4, Accessibility accessibility5, Accessibility accessibility6)
        {
            return accessibility == accessibility1
                || accessibility == accessibility2
                || accessibility == accessibility3
                || accessibility == accessibility4
                || accessibility == accessibility5
                || accessibility == accessibility6;
        }

        internal static bool ContainsProtected(this Accessibility accessibility)
        {
            return accessibility.Is(Accessibility.Protected, Accessibility.ProtectedAndInternal, Accessibility.ProtectedOrInternal);
        }

        internal static bool IsMoreRestrictiveThan(this Accessibility accessibility, Accessibility value)
        {
            switch (value)
            {
                case Accessibility.Public:
                    {
                        return accessibility == Accessibility.Internal
                            || accessibility == Accessibility.ProtectedOrInternal
                            || accessibility == Accessibility.Protected
                            || accessibility == Accessibility.Private;
                    }
                case Accessibility.Internal:
                    {
                        return accessibility == Accessibility.Private;
                    }
                case Accessibility.ProtectedOrInternal:
                    {
                        return accessibility == Accessibility.Internal
                            || accessibility == Accessibility.Protected
                            || accessibility == Accessibility.Private;
                    }
                case Accessibility.Protected:
                    {
                        return accessibility == Accessibility.Private;
                    }
                case Accessibility.Private:
                    {
                        return false;
                    }
            }

            return false;
        }

        public static string GetName(this Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    return "private";
                case Accessibility.Protected:
                    return "protected";
                case Accessibility.Internal:
                    return "internal";
                case Accessibility.ProtectedOrInternal:
                    return "protected internal";
                case Accessibility.Public:
                    return "public";
            }

            throw new ArgumentException("", nameof(accessibility));
        }
    }
}
