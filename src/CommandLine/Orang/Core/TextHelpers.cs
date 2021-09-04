// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Roslynator
{
    internal static class TextHelpers
    {
        public static IEnumerable<string> ReadLines(string value)
        {
            using (var sr = new StringReader(value))
            {
                string line = null;

                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public static string Indent(string value, int indentLength)
        {
            if (value.Contains("\n"))
            {
                var sb = new StringBuilder();

                sb.AppendIndent(value, new string(' ', indentLength));

                return sb.ToString();
            }

            return value;
        }

        public static string Indent(string value, string indent)
        {
            if (value.Contains("\n"))
            {
                var sb = new StringBuilder();

                sb.AppendIndent(value, indent);

                return sb.ToString();
            }

            return value;
        }

        internal static string Join(string separator, string lastSeparator, IEnumerable<string> values)
        {
            using (IEnumerator<string> en = values.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    var sb = new StringBuilder();

                    sb.Append(en.Current);

                    if (en.MoveNext())
                    {
                        string previous = en.Current;

                        while (true)
                        {
                            if (en.MoveNext())
                            {
                                sb.Append(separator);
                                sb.Append(previous);
                                previous = en.Current;
                            }
                            else
                            {
                                sb.Append(lastSeparator);
                                sb.Append(previous);
                                break;
                            }
                        }
                    }

                    return sb.ToString();
                }

                return "";
            }
        }

        public static int CountLines(string text, int startIndex, int length)
        {
            Debug.Assert(length >= 0, length.ToString());

            int count = 0;
            int endIndex = startIndex + length - 1;

            for (int i = startIndex; i <= endIndex; i++)
            {
                if (text[i] == '\n')
                    count++;
            }

            return count;
        }

        public static string SplitCamelCase(string value)
        {
            return SplitCamelCase(value, " ");
        }

        public static string SplitCamelCase(string value, string separator)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (separator == null)
                throw new ArgumentNullException(nameof(separator));

            int len = value.Length;
            if (len > 0)
            {
                var sb = new StringBuilder(len);
                for (int i = 0; i < len; i++)
                {
                    char ch = value[i];
                    int j = i;
                    Func<char, bool> predicate;
                    var fCaps = false;
                    var fAppend = true;
                    if (char.IsLetter(ch))
                    {
                        if (char.IsUpper(ch) && (i + 1) < len && char.IsUpper(value[i + 1]))
                        {
                            predicate = f => char.IsUpper(f);
                            fCaps = true;
                            j++;
                        }
                        else
                        {
                            predicate = f => char.IsLower(f);
                        }

                        j++;
                    }
                    else if (char.IsDigit(ch))
                    {
                        predicate = f => char.IsDigit(f);
                        j++;
                    }
                    else
                    {
                        predicate = f => !char.IsLetterOrDigit(f);
                        fAppend = false;
                    }

                    while (j < len && predicate(value[j]))
                        j++;

                    if (fAppend)
                    {
                        if (fCaps && j > (i + 1) && j < len && char.IsLower(value[j]))
                            j--;

                        if (sb.Length > 0)
                            sb.Append(separator);

                        j -= i;
                        sb.Append(value, i, j);
                        i += j - 1;
                    }
                }

                return sb.ToString();
            }

            return "";
        }
    }
}
