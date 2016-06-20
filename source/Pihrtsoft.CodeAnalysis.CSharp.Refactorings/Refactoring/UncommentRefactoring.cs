// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class UncommentRefactoring
    {
        private static readonly Regex _uncommentManyCommentsRegex = new Regex(@"
            (?<=
                ^
                [\s-[\r\n]]*
            )
            //",
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex _uncommentSingleCommentRegex = new Regex("//");

        public static async Task<Document> RefactorAsync(
            Document document,
            SyntaxTrivia comment,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxToken token = comment.Token;

            SyntaxTriviaList triviaList = default(SyntaxTriviaList);

            int index = token.LeadingTrivia.IndexOf(comment);

            if (index != -1)
            {
                triviaList = token.LeadingTrivia;
            }
            else
            {
                index = token.TrailingTrivia.IndexOf(comment);
                triviaList = token.TrailingTrivia;
            }

            string text = GetNewText(oldRoot.ToFullString(), triviaList, index);

            SyntaxTree tree = CSharpSyntaxTree.ParseText(text);

            SyntaxNode newRoot = await tree.GetRootAsync();

            return document.WithSyntaxRoot(newRoot);
        }

        private static string GetNewText(string input, SyntaxTriviaList triviaList, int index)
        {
            int firstIndex = FindFirstTriviaToRemove(triviaList, index);
            int lastIndex = FindLastTriviaToRemove(triviaList, index);
            int count = GetSingleLineCommentCount(triviaList, firstIndex, lastIndex);

            Regex regex = (count == 1) ? _uncommentSingleCommentRegex : _uncommentManyCommentsRegex;

            return regex.Replace(
                input,
                string.Empty,
                count,
                triviaList[firstIndex].SpanStart);
        }

        private static int GetSingleLineCommentCount(SyntaxTriviaList triviaList, int firstIndex, int lastIndex)
        {
            int count = 0;
            for (int i = firstIndex; i <= lastIndex; i++)
            {
                if (triviaList[i].IsKind(SyntaxKind.SingleLineCommentTrivia))
                    count++;
            }

            return count;
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
                    return true;
                case SyntaxKind.EndOfLineTrivia:
                    return true;
                case SyntaxKind.SingleLineCommentTrivia:
                    return true;
                default:
                    return false;
            }
        }
    }
}
