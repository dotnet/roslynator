// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.Helpers
{
    internal static class RemoveCommentHelper
    {
        public static Task<Document> RemoveCommentAsync(
            Document document,
            SyntaxTrivia comment,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxToken newToken = GetNewToken(comment.Token, comment)
                .WithFormatterAnnotation();

            return document.ReplaceTokenAsync(comment.Token, newToken, cancellationToken);
        }

        private static SyntaxToken GetNewToken(SyntaxToken token, SyntaxTrivia comment)
        {
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
            switch (trivia.Kind())
            {
                case SyntaxKind.WhitespaceTrivia:
                case SyntaxKind.EndOfLineTrivia:
                case SyntaxKind.SingleLineCommentTrivia:
                    return true;
                default:
                    return false;
            }
        }
    }
}
