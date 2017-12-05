// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

namespace Roslynator.Utilities.Markdown
{
    public static class MarkdownUtility
    {
        public static string Escape(string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (ShouldBeEscaped(value[i]))
                {
                    var sb = new StringBuilder();
                    sb.Append(value, 0, i);
                    sb.Append('\\');
                    sb.Append(value[i]);

                    i++;
                    int lastIndex = i;

                    while (i < value.Length)
                    {
                        if (ShouldBeEscaped(value[i]))
                        {
                            sb.Append(value, lastIndex, i - lastIndex);
                            sb.Append('\\');
                            sb.Append(value[i]);

                            i++;
                            lastIndex = i;
                        }
                        else
                        {
                            i++;
                        }
                    }

                    sb.Append(value, lastIndex, value.Length - lastIndex);

                    return sb.ToString();
                }
            }

            return value;
        }

        public static bool ShouldBeEscaped(char value)
        {
            switch (value)
            {
                case '\\':
                case '`':
                case '*':
                case '_':
                case '{':
                case '}':
                case '[':
                case ']':
                case '(':
                case ')':
                case '#':
                case '+':
                case '-':
                case '.':
                case '!':
                case '<':
                case '>':
                    return true;
                default:
                    return false;
            }
        }
    }
}
