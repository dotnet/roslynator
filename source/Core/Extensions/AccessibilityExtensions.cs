// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
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

        public static bool IsPublic(this Accessibility accessibility)
        {
            return accessibility == Accessibility.Public;
        }

        public static bool IsInternal(this Accessibility accessibility)
        {
            return accessibility == Accessibility.Internal;
        }

        public static bool IsProtectedInternal(this Accessibility accessibility)
        {
            return accessibility == Accessibility.ProtectedOrInternal;
        }

        public static bool IsProtected(this Accessibility accessibility)
        {
            return accessibility == Accessibility.Protected;
        }

        public static bool IsPrivateProtected(this Accessibility accessibility)
        {
            return accessibility == Accessibility.ProtectedAndInternal;
        }

        public static bool IsPrivate(this Accessibility accessibility)
        {
            return accessibility == Accessibility.Private;
        }

        internal static bool ContainsProtected(this Accessibility accessibility)
        {
            return accessibility.Is(Accessibility.Protected, Accessibility.ProtectedAndInternal, Accessibility.ProtectedOrInternal);
        }

        internal static AccessibilityFlags GetAccessibilityFlag(this Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.NotApplicable:
                    return AccessibilityFlags.None;
                case Accessibility.Private:
                    return AccessibilityFlags.Private;
                case Accessibility.ProtectedAndInternal:
                    return AccessibilityFlags.ProtectedAndInternal;
                case Accessibility.Protected:
                    return AccessibilityFlags.Protected;
                case Accessibility.Internal:
                    return AccessibilityFlags.Internal;
                case Accessibility.ProtectedOrInternal:
                    return AccessibilityFlags.ProtectedOrInternal;
                case Accessibility.Public:
                    return AccessibilityFlags.Public;
            }

            Debug.Fail(accessibility.ToString());

            return AccessibilityFlags.None;
        }

        internal static bool IsMoreRestrictiveThan(this Accessibility accessibility, Accessibility other)
        {
            switch (other)
            {
                case Accessibility.Public:
                    {
                        return accessibility == Accessibility.Internal
                            || accessibility == Accessibility.ProtectedOrInternal
                            || accessibility == Accessibility.ProtectedAndInternal
                            || accessibility == Accessibility.Protected
                            || accessibility == Accessibility.Private;
                    }
                case Accessibility.Internal:
                    {
                        return accessibility == Accessibility.ProtectedAndInternal
                            || accessibility == Accessibility.Private;
                    }
                case Accessibility.ProtectedOrInternal:
                    {
                        return accessibility == Accessibility.Internal
                            || accessibility == Accessibility.Protected
                            || accessibility == Accessibility.ProtectedAndInternal
                            || accessibility == Accessibility.Private;
                    }
                case Accessibility.ProtectedAndInternal:
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
                case Accessibility.ProtectedAndInternal:
                    return "private protected";
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
