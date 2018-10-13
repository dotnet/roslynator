// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Roslynator.Documentation
{
    internal static class TextUtility
    {
        private static readonly Regex _newLineAndSpacesRegex = new Regex(@"\r?\n[\s-[\r\n]]*");

        public static string RemoveLeadingTrailingNewLine(
            string s,
            bool leadingNewLine = true,
            bool trailingNewLine = true)
        {
            int length = s.Length;

            if (length == 0)
                return s;

            int startIndex = 0;

            if (leadingNewLine)
            {
                if (s[0] == '\n')
                {
                    startIndex = 1;
                }
                else if (s[0] == '\r'
                    && length > 1
                    && s[1] == '\n')
                {
                    startIndex = 2;
                }
            }

            if (trailingNewLine
                && s[length - 1] == '\n')
            {
                length--;

                if (length > 0
                    && s[length - 1] == '\r')
                {
                    length--;
                }
            }

            return s.Substring(startIndex, length - startIndex);
        }

        public static string ToSingleLine(string s, string replacement = " ")
        {
            return _newLineAndSpacesRegex.Replace(s, replacement);
        }

        internal static string RemovePrefixFromDocumentationCommentId(string id)
        {
            if (id.Length > 2
                && id[1] == ':')
            {
                // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/processing-the-xml-file
                switch (id[0])
                {
                    case 'N':
                    case 'T':
                    case 'F':
                    case 'P':
                    case 'M':
                    case 'E':
                    case '!':
                        return id.Substring(2);
                }
            }

            return id;
        }
    }
}
