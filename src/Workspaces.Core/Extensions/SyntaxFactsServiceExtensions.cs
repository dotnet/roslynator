// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class SyntaxFactsServiceExtensions
    {
        public static bool IsWhitespaceOrEndOfLineTrivia(this ISyntaxFactsService syntaxFacts, in SyntaxTrivia trivia)
        {
            return syntaxFacts.IsWhitespaceTrivia(trivia) || syntaxFacts.IsEndOfLineTrivia(trivia);
        }

        public static bool BeginsWithBanner(
            this ISyntaxFactsService syntaxFacts,
            SyntaxNode root,
            ImmutableArray<string> lines)
        {
            if (lines.Length == 0)
                return false;

            SyntaxTriviaList leading = root.GetLeadingTrivia();

            if (lines.Length > leading.Count)
                return false;

            int i = 0;
            int j = 0;

            while (i < leading.Count
                && syntaxFacts.IsWhitespaceOrEndOfLineTrivia(leading[i]))
            {
                i++;
            }

            while (i < leading.Count)
            {
                SyntaxTrivia trivia = leading[i];

                if (!syntaxFacts.IsSingleLineComment(trivia))
                    return false;

                string comment = trivia.ToString();

                if (string.CompareOrdinal(
                    lines[j],
                    0,
                    comment,
                    syntaxFacts.SingleLineCommentStart.Length,
                    comment.Length - syntaxFacts.SingleLineCommentStart.Length) != 0)
                {
                    return false;
                }

                if (j == lines.Length - 1)
                    return true;

                if (i == leading.Count - 1)
                    return false;

                i++;

                if (!syntaxFacts.IsEndOfLineTrivia(leading[i]))
                    return false;

                i++;
                j++;
            }

            return false;
        }
    }
}
