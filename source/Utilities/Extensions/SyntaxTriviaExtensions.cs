// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class SyntaxTriviaExtensions
    {
        public static SyntaxTriviaList GetContainingList(this SyntaxTrivia trivia)
        {
            SyntaxToken token = trivia.Token;

            SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

            int index = leadingTrivia.IndexOf(trivia);

            if (index != -1)
                return token.LeadingTrivia;

            SyntaxTriviaList trailingTrivia = token.TrailingTrivia;

            index = trailingTrivia.IndexOf(trivia);

            if (index != -1)
                return token.TrailingTrivia;

            Debug.Assert(false, "containing trivia list not found");

            return default(SyntaxTriviaList);
        }

        public static int GetSpanStartLine(this SyntaxTrivia trivia, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (trivia.SyntaxTree != null)
            {
                return trivia.SyntaxTree.GetLineSpan(trivia.Span, cancellationToken).StartLine();
            }
            else
            {
                return -1;
            }
        }

        public static int GetFullSpanStartLine(this SyntaxTrivia trivia, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (trivia.SyntaxTree != null)
            {
                return trivia.SyntaxTree.GetLineSpan(trivia.FullSpan, cancellationToken).StartLine();
            }
            else
            {
                return -1;
            }
        }

        public static int GetSpanEndLine(this SyntaxTrivia trivia, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (trivia.SyntaxTree != null)
            {
                return trivia.SyntaxTree.GetLineSpan(trivia.Span, cancellationToken).EndLine();
            }
            else
            {
                return -1;
            }
        }

        public static int GetFullSpanEndLine(this SyntaxTrivia trivia, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (trivia.SyntaxTree != null)
            {
                return trivia.SyntaxTree.GetLineSpan(trivia.FullSpan, cancellationToken).EndLine();
            }
            else
            {
                return -1;
            }
        }
    }
}
