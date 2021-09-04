// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Spelling
{
    internal static class TextUtility
    {
        public static int GetLineStartIndex(string input, int index)
        {
            while (index > 0
                && input[index - 1] != '\n')
            {
                index--;
            }

            return index;
        }

        public static int GetLineEndIndex(string input, int index)
        {
            while (index < input.Length)
            {
                if (input[index] == '\r'
                    || input[index] == '\n')
                {
                    return index;
                }

                index++;
            }

            return input.Length;
        }

        public static string ReplaceRange(string value, string replacement, int index, int length)
        {
            int endIndex = index + length;

            return value.Remove(index)
                + replacement
                + value.Substring(endIndex, value.Length - endIndex);
        }

        public static string SetTextCasing(string s, TextCasing textCasing)
        {
            TextCasing textCasing2 = GetTextCasing(s);

            if (textCasing == textCasing2)
                return s;

            switch (textCasing)
            {
                case TextCasing.Lower:
                    return s.ToLowerInvariant();
                case TextCasing.Upper:
                    return s.ToUpperInvariant();
                case TextCasing.FirstUpper:
                    return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1).ToLowerInvariant();
                default:
                    throw new InvalidOperationException($"Invalid enum value '{textCasing}'");
            }
        }

        public static TextCasing GetTextCasing(string s)
        {
            int length = s.Length;

            if (length == 0)
                return TextCasing.Undefined;

            char ch = s[0];

            if (char.IsLower(ch))
            {
                for (int i = 1; i < length; i++)
                {
                    if (char.IsLetter(s[i])
                        && !char.IsLower(s[i]))
                    {
                        return TextCasing.Undefined;
                    }
                }

                return TextCasing.Lower;
            }
            else if (char.IsUpper(ch))
            {
                if (length == 1)
                    return TextCasing.Upper;

                ch = s[1];

                if (char.IsLower(ch))
                {
                    for (int i = 2; i < length; i++)
                    {
                        if (char.IsLetter(s[i])
                            && !char.IsLower(s[i]))
                        {
                            return TextCasing.Undefined;
                        }
                    }

                    return TextCasing.FirstUpper;
                }
                else if (char.IsUpper(ch))
                {
                    for (int i = 0; i < length; i++)
                    {
                        if (char.IsLetter(s[i])
                            && !char.IsUpper(s[i]))
                        {
                            return TextCasing.Undefined;
                        }
                    }

                    return TextCasing.Upper;
                }
            }

            return TextCasing.Undefined;
        }

        public static bool TextCasingEquals(string value1, string value2)
        {
            return GetTextCasing(value1) == GetTextCasing(value2);
        }
    }
}
