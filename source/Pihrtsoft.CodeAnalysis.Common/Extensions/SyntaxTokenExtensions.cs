// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Pihrtsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis
{
    public static class SyntaxTokenExtensions
    {
        public static int GetSpanStartLine(this SyntaxToken token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (token.SyntaxTree != null)
                return token.SyntaxTree.GetLineSpan(token.Span, cancellationToken).StartLinePosition.Line;

            return -1;
        }

        public static int GetFullSpanStartLine(this SyntaxToken token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (token.SyntaxTree != null)
                return token.SyntaxTree.GetLineSpan(token.FullSpan, cancellationToken).StartLinePosition.Line;

            return -1;
        }

        public static int GetSpanEndLine(this SyntaxToken token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (token.SyntaxTree != null)
                return token.SyntaxTree.GetLineSpan(token.Span, cancellationToken).EndLinePosition.Line;

            return -1;
        }

        public static int GetFullSpanEndLine(this SyntaxToken token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (token.SyntaxTree != null)
                return token.SyntaxTree.GetLineSpan(token.FullSpan, cancellationToken).EndLinePosition.Line;

            return -1;
        }

        public static SyntaxToken TrimLeadingWhitespace(this SyntaxToken token)
            => token.WithLeadingTrivia(token.LeadingTrivia.TrimLeadingWhitespace());

        public static SyntaxToken TrimTrailingWhitespace(this SyntaxToken token)
            => token.WithTrailingTrivia(token.TrailingTrivia.TrimTrailingWhitespace());

        public static SyntaxTriviaList GetLeadingAndTrailingTrivia(this SyntaxToken token)
            => token.LeadingTrivia.AddRange(token.TrailingTrivia);

        public static SyntaxToken WithoutLeadingTrivia(this SyntaxToken token)
            => token.WithLeadingTrivia(SyntaxTriviaList.Empty);

        public static SyntaxToken WithoutTrailingTrivia(this SyntaxToken token)
            => token.WithTrailingTrivia(SyntaxTriviaList.Empty);

        public static SyntaxToken WithTrailingSpace(this SyntaxToken token)
            => token.WithTrailingTrivia(SyntaxFactory.Space);

        public static SyntaxToken WithTrailingNewLine(this SyntaxToken token)
            => token.WithTrailingTrivia(SyntaxHelper.NewLine);
    }
}
