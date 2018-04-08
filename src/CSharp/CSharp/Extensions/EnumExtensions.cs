// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    /// <summary>
    /// A set of extension methods for enumerations.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns true if the kind is one of the specified kinds.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <returns></returns>
        public static bool Is(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2)
        {
            return kind == kind1
                || kind == kind2;
        }

        /// <summary>
        /// Returns true if the kind is one of the specified kinds.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <returns></returns>
        public static bool Is(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        /// <summary>
        /// Returns true if the kind is one of the specified kinds.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <returns></returns>
        public static bool Is(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4;
        }

        /// <summary>
        /// Returns true if the kind is one of the specified kinds.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <returns></returns>
        public static bool Is(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5;
        }

        /// <summary>
        /// Returns true if the kind is one of the specified kinds.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="kind1"></param>
        /// <param name="kind2"></param>
        /// <param name="kind3"></param>
        /// <param name="kind4"></param>
        /// <param name="kind5"></param>
        /// <param name="kind6"></param>
        /// <returns></returns>
        public static bool Is(this SyntaxKind kind, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5, SyntaxKind kind6)
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
