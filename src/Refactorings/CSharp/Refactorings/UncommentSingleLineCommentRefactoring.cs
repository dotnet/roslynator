// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UncommentSingleLineCommentRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxTrivia singleLineComment,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxToken token = singleLineComment.Token;

            SyntaxTriviaList triviaList;

            int index = token.LeadingTrivia.IndexOf(singleLineComment);

            if (index != -1)
            {
                triviaList = token.LeadingTrivia;
            }
            else
            {
                index = token.TrailingTrivia.IndexOf(singleLineComment);
                triviaList = token.TrailingTrivia;
            }

            IEnumerable<TextChange> textChanges = GetTextChanges(triviaList, index);

            return document.WithTextChangesAsync(textChanges, cancellationToken);
        }

        private static IEnumerable<TextChange> GetTextChanges(SyntaxTriviaList triviaList, int index)
        {
            int i = index;
            while (i >= 0)
            {
                SyntaxTrivia trivia = triviaList[i];

                if (IsAllowedTrivia(trivia))
                {
                    if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                        yield return new TextChange(trivia.Span, trivia.ToString().Substring(2));

                    i--;
                }
                else
                {
                    break;
                }
            }

            i = index + 1;
            while (i < triviaList.Count)
            {
                SyntaxTrivia trivia = triviaList[i];

                if (IsAllowedTrivia(trivia))
                {
                    if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                        yield return new TextChange(trivia.Span, trivia.ToString().Substring(2));

                    i++;
                }
                else
                {
                    break;
                }
            }
        }

        private static bool IsAllowedTrivia(SyntaxTrivia trivia)
        {
            return trivia.IsKind(
                SyntaxKind.WhitespaceTrivia,
                SyntaxKind.EndOfLineTrivia,
                SyntaxKind.SingleLineCommentTrivia);
        }
    }
}
