// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp
{
    internal static class SyntaxDebug
    {
        [Conditional("DEBUG")]
        internal static void Assert(bool condition, SyntaxNode node)
        {
            if (!condition)
                Fail(node);
        }

        [Conditional("DEBUG")]
        internal static void Assert(bool condition, SyntaxToken token)
        {
            if (!condition)
                Fail(token);
        }

        [Conditional("DEBUG")]
        internal static void Assert(bool condition, SyntaxTrivia trivia)
        {
            if (!condition)
                Fail(trivia);
        }

        [Conditional("DEBUG")]
        internal static void Fail(SyntaxNode node)
        {
            TextSpan span = node.Span;
            SyntaxTriviaList leadingTrivia = node.GetLeadingTrivia();
            SyntaxTriviaList trailingTrivia = node.GetTrailingTrivia();

            if (leadingTrivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                span = TextSpan.FromBounds(node.FullSpan.Start, span.End);

            if (trailingTrivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                span = TextSpan.FromBounds(span.Start, node.FullSpan.End);

            Fail(node.ToString(span), span, node.Kind(), node.SyntaxTree);
        }

        [Conditional("DEBUG")]
        internal static void Fail(SyntaxToken token)
        {
            TextSpan span = token.Span;
            SyntaxTriviaList leadingTrivia = token.LeadingTrivia;
            SyntaxTriviaList trailingTrivia = token.TrailingTrivia;

            if (leadingTrivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                span = TextSpan.FromBounds(token.FullSpan.Start, span.End);

            if (trailingTrivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                span = TextSpan.FromBounds(span.Start, token.FullSpan.End);

            Fail(token.ToString(span), span, token.Kind(), token.SyntaxTree);
        }

        [Conditional("DEBUG")]
        internal static void Fail(SyntaxTrivia trivia)
        {
            Fail(trivia.ToFullString(), trivia.FullSpan, trivia.Kind(), trivia.SyntaxTree);
        }

        private static void Fail(
            string text,
            TextSpan span,
            SyntaxKind kind,
            SyntaxTree syntaxTree)
        {
            int maxLength = 300;
            int lineCount = 1;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    if (lineCount == 10)
                    {
                        maxLength = Math.Min(maxLength, i);
                        break;
                    }

                    lineCount++;
                }
            }

            if (text.Length > maxLength)
                text = text.Remove(maxLength) + "....." + Environment.NewLine + "<truncated>";

            FileLinePositionSpan lineSpan = syntaxTree.GetLineSpan(span);
            LinePosition startSpan = lineSpan.StartLinePosition;
            LinePosition endSpan = lineSpan.EndLinePosition;

            string message = $"Path: {syntaxTree.FilePath}"
                + Environment.NewLine
                + $"Kind: {kind}"
                + Environment.NewLine
                + $"Start L: {startSpan.Line + 1} CH: {startSpan.Character + 1}"
                + Environment.NewLine
                + $"End L: {endSpan.Line + 1} CH: {endSpan.Character + 1}"
                + Environment.NewLine
                + Environment.NewLine
                + text;

            Debug.Fail(message);
        }
    }
}
