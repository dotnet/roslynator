// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;

namespace Roslynator
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

        public static bool StartsWithLowerLetter(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Length > 0
                && char.IsLetter(value[0])
                && char.IsLower(value[0]);
        }

        public static bool StartsWithUpperLetter(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Length > 0
                && char.IsLetter(value[0])
                && char.IsUpper(value[0]);
        }

        public static bool IsWhitespace(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                    return false;
            }

            return true;
        }

        public static string GetIndent(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (char ch in value)
            {
                if (ch == '\n'
                    || ch == '\r'
                    || !char.IsWhiteSpace(ch))
                {
                    break;
                }

                sb.Append(ch);
            }

            return sb.ToString();
        }

        public static string ToCamelCaseWithUnderscore(string value)
        {
            return CreateName(value, "_");
        }

        public static string ToCamelCase(string value)
        {
            return CreateName(value, "");
        }

        public static string ToCamelCase(string value, bool prefixWithUnderscore = false)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            string prefix = (prefixWithUnderscore) ? "_" : "";

            if (value.Length == 0)
                return prefix;

            return CreateName(value, prefix);
        }

        private static string CreateName(string value, string prefix)
        {
            var sb = new StringBuilder(prefix, value.Length + prefix.Length);

            int i = 0;

            while (i < value.Length && value[i] == '_')
                i++;

            if (char.IsUpper(value[i]))
            {
                sb.Append(char.ToLower(value[i]));
            }
            else
            {
                sb.Append(value[i]);
            }

            i++;

            sb.Append(value, i, value.Length - i);

            return sb.ToString();
        }

        public static bool IsValidCamelCaseWithUnderscore(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value[0] == '_')
            {
                if (value.Length > 1)
                {
                    return value[1] != '_'
                        && !char.IsUpper(value[1]);
                }

                return true;
            }

            return false;
        }

        public static bool IsValidCamelCaseWithoutUnderscore(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Length > 0
                && value[0] != '_'
                && char.IsLower(value[0]);
        }

        public static bool HasPrefix(string value, string prefix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));

            if (prefix.Length == 0 || value.Length <= prefix.Length)
            {
                return false;
            }

            return value.StartsWith(prefix, StringComparison.Ordinal)
                && !char.IsLower(value[prefix.Length]);
        }

        public static bool HasSuffix(string value, string suffix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (suffix == null)
                throw new ArgumentNullException(nameof(suffix));

            if (suffix.Length == 0 || value.Length <= suffix.Length)
            {
                return false;
            }

            return value.EndsWith(suffix, StringComparison.Ordinal)
                && !char.IsUpper(value[value.Length - suffix.Length - 1]);
        }

        public static string RemovePrefix(string value, string prefix)
        {
            if (HasPrefix(value, prefix))
                return value.Substring(prefix.Length);

            return value;
        }

        public static string RemoveSuffix(string value, string suffix)
        {
            if (HasSuffix(value, suffix))
                return value.Remove(value.Length - suffix.Length);

            return value;
        }
    }
}
