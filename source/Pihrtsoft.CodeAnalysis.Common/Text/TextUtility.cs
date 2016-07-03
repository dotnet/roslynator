// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Pihrtsoft.CodeAnalysis.Text
{
    public static class TextUtility
    {
        public static string Spaces(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "String length cannot be less than zero.");

            return new string(' ', count);
        }

        public static string FirstCharToLower(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                return value;

            return char.ToLower(value[0]) + value.Substring(1);
        }

        public static string FirstCharToLowerInvariant(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                return value;

            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        public static string FirstCharToUpper(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                return value;

            return char.ToUpper(value[0]) + value.Substring(1);
        }

        public static string FirstCharToUpperInvariant(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                return value;

            return char.ToUpperInvariant(value[0]) + value.Substring(1);
        }

        public static bool StartsWithLowercaseLetter(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Length > 0
                && char.IsLetter(value[0])
                && char.IsLower(value[0]);
        }
    }
}
