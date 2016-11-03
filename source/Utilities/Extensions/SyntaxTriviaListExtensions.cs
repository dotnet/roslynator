// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator
{
    public static class SyntaxTriviaListExtensions
    {
        public static bool IsEmpty(this SyntaxTriviaList list)
        {
            return list.Count == 0;
        }

        public static bool Contains(this SyntaxTriviaList list, SyntaxKind kind)
        {
            return list.IndexOf(kind) != -1;
        }

        public static SyntaxTriviaList TrimStart(this SyntaxTriviaList list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsKind(SyntaxKind.WhitespaceTrivia))
                    continue;

                if (list[i].IsKind(SyntaxKind.EndOfLineTrivia))
                    continue;

                return TriviaList(list.Skip(i));
            }

            return SyntaxTriviaList.Empty;
        }

        public static SyntaxTriviaList TrimEnd(this SyntaxTriviaList list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].IsKind(SyntaxKind.WhitespaceTrivia))
                    continue;

                if (list[i].IsKind(SyntaxKind.EndOfLineTrivia))
                    continue;

                return TriviaList(list.Take(i + 1));
            }

            return SyntaxTriviaList.Empty;
        }

        public static SyntaxTriviaList Trim(this SyntaxTriviaList list)
        {
            int startIndex = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    startIndex = i;
                }
            }

            int endIndex = list.Count;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!list[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    endIndex = i;
                }
            }

            return TriviaList(list.Skip(startIndex).Take(endIndex + 1 - startIndex));
        }
    }
}
