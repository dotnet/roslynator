// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp;

internal static class RemoveCommentHelper
{
    public static SyntaxToken GetReplacementToken(SyntaxTrivia comment)
    {
        SyntaxDebug.Assert(CSharpFacts.IsCommentTrivia(comment.Kind()), comment);

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

        return triviaList.RemoveRange(first, last - first + 1);
    }

    private static int FindFirstTriviaToRemove(SyntaxTriviaList triviaList, int index)
    {
        int firstIndex = index;

        while (index > 0)
        {
            SyntaxKind kind = triviaList[index - 1].Kind();

            if (kind == SyntaxKind.SingleLineCommentTrivia)
            {
                index--;
                firstIndex = index;
            }
            else if (kind == SyntaxKind.WhitespaceTrivia)
            {
                index--;

                if (index == 0)
                    return index;
            }
            else if (kind == SyntaxKind.EndOfLineTrivia)
            {
                index--;

                if (index == 0)
                    return index;
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

        while (index < triviaList.Count - 1
            && triviaList[index + 1].Kind().Is(
                SyntaxKind.WhitespaceTrivia,
                SyntaxKind.EndOfLineTrivia,
                SyntaxKind.SingleLineCommentTrivia))
        {
            index++;
        }

        if (index > lastIndex)
        {
            if (triviaList[index].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                if (index > lastIndex - 1
                    && triviaList[index - 1].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    index -= 2;
                }
            }
            else if (index > lastIndex
                && triviaList[index].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                index--;
            }
        }

        return index;
    }
}
