// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Text
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class SyntaxNodeTextBuilder
    {
        public SyntaxNodeTextBuilder(SyntaxNode node)
            : this(node, new StringBuilder())
        {
        }

        public SyntaxNodeTextBuilder(SyntaxNode node, StringBuilder sb)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            StringBuilder = sb ?? throw new ArgumentNullException(nameof(sb));

            FullSpan = Node.FullSpan;
            FullString = node.ToFullString();
        }

        public SyntaxNode Node { get; }

        public StringBuilder StringBuilder { get; }

        public TextSpan FullSpan { get; }

        public string FullString { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return ToString(); }
        }

        public override string ToString()
        {
            return StringBuilder.ToString();
        }

        public void AppendSpan()
        {
            AppendImpl(Node.Span);
        }

        public void AppendSpan(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            AppendImpl(node.Span);
        }

        public void AppendSpan(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            AppendImpl(token.Span);
        }

        public void AppendSpan(SyntaxTrivia trivia)
        {
            ThrowIfInvalid(trivia);

            AppendImpl(trivia.Span);
        }

        public void AppendFullSpan()
        {
            AppendImpl(Node.FullSpan);
        }

        public void AppendFullSpan(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            AppendImpl(node.FullSpan);
        }

        public void AppendFullSpan(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            AppendImpl(token.FullSpan);
        }

        public void AppendFullSpan(SyntaxTrivia trivia)
        {
            ThrowIfInvalid(trivia);

            AppendImpl(trivia.FullSpan);
        }

        public void AppendLeadingTrivia()
        {
            AppendImpl(Node.LeadingTriviaSpan());
        }

        public void AppendLeadingTrivia(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            AppendImpl(node.LeadingTriviaSpan());
        }

        public void AppendLeadingTrivia(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            AppendImpl(token.LeadingTriviaSpan());
        }

        public void AppendLeadingTrivia(SyntaxTrivia trivia)
        {
            ThrowIfInvalid(trivia);

            AppendImpl(trivia.LeadingTriviaSpan());
        }

        public void AppendLeadingTriviaAndSpan()
        {
            AppendLeadingTrivia();
            AppendSpan();
        }

        public void AppendLeadingTriviaAndSpan(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            AppendImpl(node.LeadingTriviaSpan());
            AppendImpl(node.Span);
        }

        public void AppendLeadingTriviaAndSpan(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            AppendImpl(token.LeadingTriviaSpan());
            AppendImpl(token.Span);
        }

        public void AppendLeadingTriviaAndSpan(SyntaxTrivia trivia)
        {
            ThrowIfInvalid(trivia);

            AppendImpl(trivia.LeadingTriviaSpan());
            AppendImpl(trivia.Span);
        }

        public void AppendTrailingTrivia()
        {
            AppendImpl(Node.TrailingTriviaSpan());
        }

        public void AppendTrailingTrivia(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            AppendImpl(node.TrailingTriviaSpan());
        }

        public void AppendTrailingTrivia(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            AppendImpl(token.TrailingTriviaSpan());
        }

        public void AppendTrailingTrivia(SyntaxTrivia trivia)
        {
            ThrowIfInvalid(trivia);

            AppendImpl(trivia.TrailingTriviaSpan());
        }

        public void AppendSpanAndTrailingTrivia()
        {
            AppendSpan();
            AppendTrailingTrivia();
        }

        public void AppendSpanAndTrailingTrivia(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            AppendImpl(node.Span);
            AppendImpl(node.TrailingTriviaSpan());
        }

        public void AppendSpanAndTrailingTrivia(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            AppendImpl(token.Span);
            AppendImpl(token.TrailingTriviaSpan());
        }

        public void AppendSpanAndTrailingTrivia(SyntaxTrivia trivia)
        {
            ThrowIfInvalid(trivia);

            AppendImpl(trivia.Span);
            AppendImpl(trivia.TrailingTriviaSpan());
        }

        public void Append(string value)
        {
            StringBuilder.Append(value);
        }

        public void Append(char value, int repeatCount)
        {
            StringBuilder.Append(value, repeatCount);
        }

        public void AppendLine()
        {
            StringBuilder.AppendLine();
        }

        public void AppendLine(string value)
        {
            StringBuilder.AppendLine(value);
        }

        public void Append(TextSpan span)
        {
            ThrowIfInvalid(span);
            AppendImpl(span);
        }

        private void AppendImpl(TextSpan span)
        {
            StringBuilder.Append(FullString, span.Start - FullSpan.Start, span.Length);
        }

        private void ThrowIfInvalid(SyntaxNode node)
        {
            if (!FullSpan.Contains(node.FullSpan))
                throw new ArgumentException("", nameof(node));
        }

        private void ThrowIfInvalid(SyntaxToken token)
        {
            if (!FullSpan.Contains(token.FullSpan))
                throw new ArgumentException("", nameof(token));
        }

        private void ThrowIfInvalid(SyntaxTrivia trivia)
        {
            if (!FullSpan.Contains(trivia.FullSpan))
                throw new ArgumentException("", nameof(trivia));
        }

        private void ThrowIfInvalid(TextSpan span)
        {
            if (!FullSpan.Contains(span))
                throw new ArgumentException("", nameof(span));
        }
    }
}
