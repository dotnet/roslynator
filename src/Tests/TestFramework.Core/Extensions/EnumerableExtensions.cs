// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class EnumerableExtensions
    {
        public static string ToDebugString(this IEnumerable<Diagnostic> diagnostics)
        {
            string s = string.Join("\r\n", diagnostics.Select(d => d.ToString()));

            if (s.Length == 0)
                s = "no diagnostic";

            return $"\r\n\r\nDiagnostics:\r\n{s}\r\n";
        }
    }
}
