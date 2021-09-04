// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Roslynator
{
    internal static class CoreExtensions
    {
        public static StringBuilder AppendIndent(this StringBuilder sb, string value, int indentLength)
        {
            if (value.Contains("\n"))
            {
                var indent = new string(' ', indentLength);

                return AppendIndent(sb, value, indent);
            }
            else
            {
                return sb.Append(value);
            }
        }

        public static StringBuilder AppendIndent(this StringBuilder sb, string value, string indent)
        {
            using (IEnumerator<string> en = TextHelpers.ReadLines(value).GetEnumerator())
            {
                if (en.MoveNext())
                {
                    sb.Append(en.Current);

                    while (en.MoveNext())
                    {
                        sb.AppendLine();
                        sb.Append(indent);
                        sb.Append(en.Current);
                    }
                }
            }

            return sb;
        }

        public static StringBuilder AppendSpaces(this StringBuilder sb, int count)
        {
            return sb.Append(' ', count);
        }

        public static int EndIndex(this Capture capture)
        {
            return capture.Index + capture.Length;
        }

        public static int GetDigitCount(this int value)
        {
            if (value < 0)
                value = -value;

            if (value < 10)
                return 1;

            if (value < 100)
                return 2;

            if (value < 1000)
                return 3;

            if (value < 10000)
                return 4;

            if (value < 100000)
                return 5;

            if (value < 1000000)
                return 6;

            if (value < 10000000)
                return 7;

            if (value < 100000000)
                return 8;

            if (value < 1000000000)
                return 9;

            return 10;
        }

        public static T SingleOrDefault<T>(this IReadOnlyCollection<T> values, bool shouldThrow)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (shouldThrow)
            {
                return values.SingleOrDefault();
            }
            else
            {
                return (values.Count == 1) ? values.First() : default;
            }
        }

        public static T SingleOrDefault<T>(
            this IReadOnlyCollection<T> list,
            Func<T, bool> predicate,
            bool shouldThrow)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (shouldThrow)
                return list.SingleOrDefault(predicate);

            using (IEnumerator<T> en = list.GetEnumerator())
            {
                while (en.MoveNext())
                {
                    T item = en.Current;

                    if (predicate(item))
                    {
                        while (en.MoveNext())
                        {
                            if (predicate(en.Current))
                                return default;
                        }

                        return item;
                    }
                }
            }

            return default;
        }
    }
}
