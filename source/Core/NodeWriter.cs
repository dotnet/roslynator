// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    internal class NodeWriter
    {
        private readonly string _text;
        private readonly TextSpan _fullSpan;

        public NodeWriter(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            Node = node;

            _text = node.ToFullString();
            _fullSpan = node.FullSpan;
        }

        public SyntaxNode Node { get; }

        public StringBuilder StringBuilder { get; } = new StringBuilder();

        public override string ToString()
        {
            return StringBuilder.ToString();
        }

        public void WriteSpan()
        {
            Write(Node.Span);
        }

        public void WriteSpan(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            Write(node.Span);
        }

        public void WriteSpan(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            Write(token.Span);
        }

        public void WriteFullSpan()
        {
            Write(Node.FullSpan);
        }

        public void WriteFullSpan(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            Write(node.FullSpan);
        }

        public void WriteFullSpan(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            Write(token.FullSpan);
        }

        public void WriteLeadingTrivia()
        {
            Write(Node.LeadingTriviaSpan());
        }

        public void WriteLeadingTrivia(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            Write(node.LeadingTriviaSpan());
        }

        public void WriteLeadingTrivia(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            Write(token.LeadingTriviaSpan());
        }

        public void WriteLeadingTriviaAndSpan()
        {
            WriteLeadingTrivia();
            WriteSpan();
        }

        public void WriteLeadingTriviaAndSpan(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            Write(node.LeadingTriviaSpan());
            Write(node.Span);
        }

        public void WriteLeadingTriviaAndSpan(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            Write(token.LeadingTriviaSpan());
            Write(token.Span);
        }

        public void WriteTrailingTrivia()
        {
            Write(Node.TrailingTriviaSpan());
        }

        public void WriteTrailingTrivia(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            Write(node.TrailingTriviaSpan());
        }

        public void WriteTrailingTrivia(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            Write(token.TrailingTriviaSpan());
        }

        public void WriteSpanAndTrailingTrivia()
        {
            WriteSpan();
            WriteTrailingTrivia();
        }

        public void WriteSpanAndTrailingTrivia(SyntaxNode node)
        {
            ThrowIfInvalid(node);

            Write(node.Span);
            Write(node.TrailingTriviaSpan());
        }

        public void WriteSpanAndTrailingTrivia(SyntaxToken token)
        {
            ThrowIfInvalid(token);

            Write(token.Span);
            Write(token.TrailingTriviaSpan());
        }

        public void Write(string value)
        {
            StringBuilder.Append(value);
        }

        public void WriteLine()
        {
            StringBuilder.AppendLine();
        }

        public void WriteLine(string value)
        {
            StringBuilder.AppendLine(value);
        }

        private void Write(TextSpan span)
        {
            StringBuilder.Append(_text, span.Start - _fullSpan.Start, span.Length);
        }

        private void ThrowIfInvalid(SyntaxNode node)
        {
            if (!_fullSpan.Contains(node.FullSpan))
                throw new ArgumentException("", nameof(node));
        }

        private void ThrowIfInvalid(SyntaxToken token)
        {
            if (!_fullSpan.Contains(token.FullSpan))
                throw new ArgumentException("", nameof(token));
        }
    }
}
