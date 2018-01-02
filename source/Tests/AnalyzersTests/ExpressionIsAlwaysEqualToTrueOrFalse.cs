// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1118, RCS1176

using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class ExpressionIsAlwaysEqualToTrueOrFalse
    {
        public static void Foo()
        {
            byte b = 0;
            ushort us = 0;
            uint ui = 0;
            ulong ul = 0;
            var items = new List<string>();
            var arr = new string[0];
            string s = null;

            // true

            if (b >= 0) { }

            if (0 <= b) { }

            if (us >= 0) { }

            if (0 <= us) { }

            if (ui >= 0) { }

            if (0 <= ui) { }

            if (ul >= 0) { }

            if (0 <= ul) { }

            if (items.Count >= 0) { }

            if (0 <= items.Count) { }

            if (arr.Length >= 0) { }

            if (0 <= arr.Length) { }

            if (s.Length >= 0) { }

            if (0 <= s.Length) { }

            // false

            if (b < 0) { }

            if (0 > b) { }

            if (us < 0) { }

            if (0 > us) { }

            if (ui < 0) { }

            if (0 > ui) { }

            if (ul < 0) { }

            if (0 > ul) { }

            if (items.Count < 0) { }

            if (0 > items.Count) { }

            if (arr.Length < 0) { }

            if (0 > arr.Length) { }

            if (s.Length < 0) { }

            if (0 > s.Length) { }

            // n

            for (int i = items.Count - 1; i >= 0; i--)
            {
            }
        }
    }
}
