// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;

namespace Roslynator
{
    public static class IdentifierUtility
    {
        public static string ToCamelCase(string value, bool prefixWithUnderscore = false)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            string prefix = (prefixWithUnderscore) ? "_" : "";

            if (value.Length > 0)
            {
                return ToCamelCase(value, prefix);
            }
            else
            {
                return prefix;
            }
        }

        private static string ToCamelCase(string value, string prefix)
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

        public static bool IsCamelCasePrefixedWithUnderscore(string value)
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

        public static bool IsCamelCaseNotPrefixedWithUnderscore(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Length > 0
                && value[0] != '_'
                && char.IsLower(value[0]);
        }

        public static bool HasPrefix(string value, string prefix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));

            return prefix.Length > 0
                && value.Length > prefix.Length
                && value.StartsWith(prefix, comparison)
                && IsBoundary(value[prefix.Length - 1], value[prefix.Length]);
        }

        public static bool HasSuffix(string value, string suffix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (suffix == null)
                throw new ArgumentNullException(nameof(suffix));

            return suffix.Length > 0
                && value.Length > suffix.Length
                && value.EndsWith(suffix, comparison)
                && IsBoundary(value[value.Length - suffix.Length - 1], value[value.Length - suffix.Length]);
        }

        private static bool IsBoundary(char ch1, char ch2)
        {
            if (IsHyphenOrUnderscore(ch1))
            {
                return !IsHyphenOrUnderscore(ch2);
            }
            else if (char.IsDigit(ch1))
            {
                return IsHyphenOrUnderscore(ch2);
            }
            else if (char.IsLower(ch1))
            {
                return IsHyphenOrUnderscore(ch2) || char.IsUpper(ch2);
            }
            else
            {
                return IsHyphenOrUnderscore(ch2);
            }
        }

        private static bool IsHyphenOrUnderscore(char ch)
        {
            return ch == '-' || ch == '_';
        }

        public static string RemovePrefix(string value, string prefix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (HasPrefix(value, prefix, comparison))
            {
                return value.Substring(prefix.Length);
            }
            else
            {
                return value;
            }
        }

        public static string RemoveSuffix(string value, string suffix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (HasSuffix(value, suffix, comparison))
            {
                return value.Remove(value.Length - suffix.Length);
            }
            else
            {
                return value;
            }
        }
    }
}
