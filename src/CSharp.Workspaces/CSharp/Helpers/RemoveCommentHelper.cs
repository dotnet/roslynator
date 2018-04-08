// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.Helpers
{
    internal static class RemoveCommentHelper
    {
        public static SyntaxToken GetReplacementToken(SyntaxTrivia comment)
        {
            Debug.Assert(CSharpFacts.IsCommentTrivia(comment.Kind()), comment.Kind().ToString());

            SyntaxToken token = comment.Token;

            int index = token.LeadingTrivia.IndexOf(comment);

            if (index != -1)
            {
                if (comment.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    return token.WithLeadingTrivia(RemoveTrivia(token.LeadingTrivia, index));

                return token.WithLeadingTrivia(token.LeadingTrivia.RemoveAt(index));
            }

            index = token.TrailingTrivia.IndexOf(comment);

            if (index != -1)
            {
                if (comment.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    return token.WithTrailingTrivia(RemoveTrivia(token.TrailingTrivia, index));

                return token.WithTrailingTrivia(token.TrailingTrivia.RemoveAt(index));
            }

            Debug.Fail("comment trivia not found");

            return token;
        }

        private static SyntaxTriviaList RemoveTrivia(SyntaxTriviaList triviaList, int index)
        {
            int first = FindFirstTriviaToRemove(triviaList, index);
            int last = FindLastTriviaToRemove(triviaList, index);

            for (int i = last; i >= first; i--)
                triviaList = triviaList.RemoveAt(i);

            return triviaList;
        }

        private static int FindFirstTriviaToRemove(SyntaxTriviaList triviaList, int index)
        {
            int firstIndex = index;

            while (index > 0)
            {
                if (IsAllowedTrivia(triviaList[index - 1]))
                {
                    index--;

                    if (triviaList[index].IsKind(SyntaxKind.SingleLineCommentTrivia))
                        firstIndex = index;
                }
                else
                {
                    break;
                }
            }

            return firstIndex;
        }

        private static int FindLastTriviaToRemove(SyntaxTriviaList triviaList, int index)
        {
            int lastIndex = index;

            while (index < triviaList.Count - 1)
            {
                if (IsAllowedTrivia(triviaList[index + 1]))
                {
                    index++;

                    if (triviaList[index].IsKind(SyntaxKind.SingleLineCommentTrivia))
                        lastIndex = index;
                }
                else
                {
                    break;
                }
            }

            return lastIndex;
        }

        private static bool IsAllowedTrivia(SyntaxTrivia trivia)
        {
            return trivia.Kind().Is(
                SyntaxKind.WhitespaceTrivia,
                SyntaxKind.EndOfLineTrivia,
                SyntaxKind.SingleLineCommentTrivia);
        }
    }
}
