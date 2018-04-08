// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Text
{
    internal class SyntaxNodeTextBuilder
    {
        private readonly string _text;

        public SyntaxNodeTextBuilder(SyntaxNode node)
            : this(node, new StringBuilder())
        {
        }

        public SyntaxNodeTextBuilder(SyntaxNode node, StringBuilder stringBuilder)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            StringBuilder = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));

            FullSpan = Node.FullSpan;
            _text = node.ToFullString();
        }

        public SyntaxNode Node { get; }

        public StringBuilder StringBuilder { get; }

        public TextSpan FullSpan { get; }

        public override string ToString()
        {
            return StringBuilder.ToString();
        }

        public void AppendSpan()
        {
            Append(Node.Span);
        }

        public void AppendSpan(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            Append(node.Span);
        }

        public void AppendSpan(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            Append(token.Span);
        }

        public void AppendSpan(SyntaxTrivia trivia)
        {
            ThrowIfInvalid(trivia);

            Append(trivia.Span);
        }

        public void AppendFullSpan()
        {
            Append(Node.FullSpan);
        }

        public void AppendFullSpan(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            Append(node.FullSpan);
        }

        public void AppendFullSpan(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            Append(token.FullSpan);
        }

        public void AppendFullSpan(SyntaxTrivia trivia)
        {
            ThrowIfInvalid(trivia);

            Append(trivia.FullSpan);
        }

        public void AppendLeadingTrivia()
        {
            Append(Node.LeadingTriviaSpan());
        }

        public void AppendLeadingTrivia(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            Append(node.LeadingTriviaSpan());
        }

        public void AppendLeadingTrivia(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            Append(token.LeadingTriviaSpan());
        }

        public void AppendLeadingTrivia(SyntaxTrivia trivia)
        {
            ThrowIfInvalid(trivia);

            Append(trivia.LeadingTriviaSpan());
        }

        public void AppendLeadingTriviaAndSpan()
        {
            AppendLeadingTrivia();
            AppendSpan();
        }

        public void AppendLeadingTriviaAndSpan(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            Append(node.LeadingTriviaSpan());
            Append(node.Span);
        }

        public void AppendLeadingTriviaAndSpan(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            Append(token.LeadingTriviaSpan());
            Append(token.Span);
        }

        public void AppendLeadingTriviaAndSpan(SyntaxTrivia trivia)
        {
            ThrowIfInvalid(trivia);

            Append(trivia.LeadingTriviaSpan());
            Append(trivia.Span);
        }

        public void AppendTrailingTrivia()
        {
            Append(Node.TrailingTriviaSpan());
        }

        public void AppendTrailingTrivia(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            Append(node.TrailingTriviaSpan());
        }

        public void AppendTrailingTrivia(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            Append(token.TrailingTriviaSpan());
        }

        public void AppendTrailingTrivia(SyntaxTrivia trivia)
        {
            ThrowIfInvalid(trivia);

            Append(trivia.TrailingTriviaSpan());
        }

        public void AppendSpanAndTrailingTrivia()
        {
            AppendSpan();
            AppendTrailingTrivia();
        }

        public void AppendSpanAndTrailingTrivia(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            Append(node.Span);
            Append(node.TrailingTriviaSpan());
        }

        public void AppendSpanAndTrailingTrivia(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            Append(token.Span);
            Append(token.TrailingTriviaSpan());
        }

        public void AppendSpanAndTrailingTrivia(SyntaxTrivia trivia)
        {
            ThrowIfInvalid(trivia);

            Append(trivia.Span);
            Append(trivia.TrailingTriviaSpan());
        }

        public void Append(string value)
        {
            StringBuilder.Append(value);
        }

        public void AppendLine()
        {
            StringBuilder.AppendLine();
        }

        public void AppendLine(string value)
        {
            StringBuilder.AppendLine(value);
        }

        private void Append(TextSpan span)
        {
            StringBuilder.Append(_text, span.Start - FullSpan.Start, span.Length);
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
    }
}
