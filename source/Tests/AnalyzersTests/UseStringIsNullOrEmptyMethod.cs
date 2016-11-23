// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseStringIsNullOrEmptyMethod
    {
        public static string Value { get; }

        public static void Foo()
        {
            string s = null;
            string s2 = null;

            if (s == null || s.Length == 0)
            {
            }

            if (s != null && s.Length != 0)
            {
            }

            if (s != null && s.Length > 0)
            {
            }

            if (Value == null || Value.Length == 0)
            {
            }

            if (Value != null && Value.Length != 0)
            {
            }

            if (Value != null && Value.Length > 0)
            {
            }

            if (s2 == null || s.Length == 0)
            {
            }

            if (s != null || s.Length == 0)
            {
            }

            if (s == s2 || s.Length == 0)
            {
            }

            if (s == null && s.Length == 0)
            {
            }

            if (s == null || s2.Length == 0)
            {
            }

            if (s == null || s.Length != 0)
            {
            }

            if (s == null || s.Length == 1)
            {
            }
        }
    }
}
