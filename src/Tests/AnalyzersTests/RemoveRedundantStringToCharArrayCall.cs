// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class RemoveRedundantStringToCharArrayCall
    {
        public static void Foo()
        {
            string s = null;

            char firstChar = s.ToCharArray()[0];

            foreach (char ch in s.ToCharArray())
            {
            }

            foreach (char ch in (s.ToCharArray()))
            {
            }

            //n

            char[] chars = s.ToCharArray();
        }
    }
}
