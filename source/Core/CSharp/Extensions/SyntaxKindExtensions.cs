// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    public static class SyntaxKindExtensions
    {
        internal static bool IsKind(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2)
        {
            return kind == kind1
                || kind == kind2;
        }

        public static bool IsKind(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        public static bool IsKind(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4;
        }

        public static bool IsKind(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5;
        }

        public static bool IsKind(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5, SyntaxKind kind6)
        {
            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5
                || kind == kind6;
        }
    }
}
