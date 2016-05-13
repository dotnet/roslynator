// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis
{
    public static class SyntaxTriviaListExtensions
    {
        public static bool IsEmpty(this SyntaxTriviaList list)
            => list.Count == 0;

        public static bool Contains(this SyntaxTriviaList list, SyntaxKind kind)
            => list.IndexOf(kind) != -1;

        public static bool ContainsEndOfLine(this SyntaxTriviaList list)
            => Contains(list, SyntaxKind.EndOfLineTrivia);

        public static SyntaxTriviaList TrimLeadingWhitespace(this SyntaxTriviaList list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsKind(SyntaxKind.WhitespaceTrivia))
                    continue;

                if (list[i].IsKind(SyntaxKind.EndOfLineTrivia))
                    continue;

                return SyntaxFactory.TriviaList(list.Skip(i));
            }

            return SyntaxTriviaList.Empty;
        }

        public static SyntaxTriviaList TrimTrailingWhitespace(this SyntaxTriviaList list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].IsKind(SyntaxKind.WhitespaceTrivia))
                    continue;

                if (list[i].IsKind(SyntaxKind.EndOfLineTrivia))
                    continue;

                return SyntaxFactory.TriviaList(list.Take(i + 1));
            }

            return SyntaxTriviaList.Empty;
        }

        public static SyntaxTriviaList TrimWhitespace(this SyntaxTriviaList list)
        {
            int startIndex = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].IsKind(SyntaxKind.WhitespaceTrivia)
                    && !list[i].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    startIndex = i;
                }
            }

            int endIndex = list.Count;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!list[i].IsKind(SyntaxKind.WhitespaceTrivia)
                    && !list[i].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    endIndex = i;
                }
            }

            return SyntaxFactory.TriviaList(list.Skip(startIndex).Take(endIndex + 1 - startIndex));
        }

        public static bool IsWhitespaceOrEndOfLine(this SyntaxTriviaList triviaList)
        {
            foreach (SyntaxTrivia trivia in triviaList)
            {
                if (!trivia.IsKind(SyntaxKind.WhitespaceTrivia)
                    && !trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
