// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

namespace Roslynator.Text
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder Replace(this StringBuilder sb, char oldChar, char newChar, int startIndex)
        {
            return sb.Replace(oldChar, newChar, startIndex, sb.Length - startIndex);
        }

        public static StringBuilder Replace(this StringBuilder sb, string oldValue, string newValue, int startIndex)
        {
            return sb.Replace(oldValue, newValue, startIndex, sb.Length - startIndex);
        }
    }
}
