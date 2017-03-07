// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Test
{
    public static class UseConditionalAccess
    {
        private static void Foo()
        {
            string s = null;

            if (s != null && s.StartsWith("a"))
            {
            }

            if (s != null && s.Length > 0)
            {
            }

            if (s != null && !s.StartsWith("a"))
            {
            }

            Dictionary<int, string> dic = null;

            if (dic != null && dic[0].StartsWith("a"))
            {
            }

            if (dic != null && dic[0].Length > 0)
            {
            }

            if (dic != null && !dic[0].StartsWith("a"))
            {
            }

            if (s != null && s.Substring(0) == null)
            {
            }
        }
    }
}
