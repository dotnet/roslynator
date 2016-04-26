// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis
{
    public static class SyntaxTriviaExtensions
    {
        public static bool IsAnyKind(this SyntaxTrivia syntaxTrivia, params SyntaxKind[] syntaxKinds)
        {
            if (syntaxKinds == null)
                throw new ArgumentNullException(nameof(syntaxKinds));

            for (int i = 0; i < syntaxKinds.Length; i++)
            {
                if (syntaxTrivia.IsKind(syntaxKinds[i]))
                    return true;
            }

            return false;
        }

        public static bool IsAnyKind(this SyntaxTrivia syntaxTrivia, IEnumerable<SyntaxKind> syntaxKinds)
        {
            if (syntaxKinds == null)
                throw new ArgumentNullException(nameof(syntaxKinds));

            foreach (SyntaxKind syntaxKind in syntaxKinds)
            {
                if (syntaxTrivia.IsKind(syntaxKind))
                    return true;
            }

            return false;
        }

        public static bool IsWhitespaceOrEndOfLine(this SyntaxTrivia syntaxTrivia)
        {
            if (syntaxTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                return true;

            if (syntaxTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                return true;

            return false;
        }

        public static bool IsWhitespace(this SyntaxTrivia syntaxTrivia)
            => syntaxTrivia.IsKind(SyntaxKind.WhitespaceTrivia);

        public static bool IsEndOfLine(this SyntaxTrivia syntaxTrivia)
            => syntaxTrivia.IsKind(SyntaxKind.EndOfLineTrivia);
    }
}
