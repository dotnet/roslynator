// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp
{
    internal static class FormattingVerifier
    {
        [Conditional("DEBUG")]
        public static void VerifyChangedSpansAreWhitespace(SyntaxNode node, IList<TextChange> textChanges)
        {
            Debug.Assert(textChanges.Count > 0, $"'{nameof(textChanges)}' is empty\r\n\r\n{node}");

            foreach (TextChange textChange in textChanges)
            {
                if (!VerifyTextChange(textChange))
                {
                    Debug.Fail("Cannot find matching trivia for TextChange.\r\n"
                        + $"span: {textChange.Span}\r\n"
                        + $"new text: {textChange.NewText}\r\n"
                        + $"new text length: {textChange.NewText.Length}\r\n"
                        + $"node:\r\n{node}");

                    break;
                }
            }

            bool VerifyTextChange(TextChange textChange)
            {
                TextSpan span = textChange.Span;
                int start = span.Start;
                int end = span.End;

                while (!node.FullSpan.Contains(start)
                    || !node.FullSpan.Contains(end))
                {
                    node = node.Parent;
                }

                SyntaxToken token = node.FindToken(start);

                if (span.IsEmpty)
                {
                    return start == token.SpanStart
                        || start == token.Span.End;
                }

                SyntaxTriviaList leading = token.LeadingTrivia;

                if (leading.Span.Contains(start))
                {
                    return end <= leading.Span.End
                        && VerifySpan(span, leading);
                }

                SyntaxTriviaList trailing = token.TrailingTrivia;

                if (!trailing.Span.Contains(start))
                    return false;

                if (end <= trailing.Span.End)
                    return VerifySpan(span, trailing);

                if (!VerifySpan(
                    TextSpan.FromBounds(start, trailing.Span.End),
                    trailing))
                {
                    return false;
                }

                leading = node.FindToken(end).LeadingTrivia;

                if (trailing.Span.End != leading.Span.End)
                    return false;

                return VerifySpan(
                    TextSpan.FromBounds(leading.Span.Start, end),
                    leading);
            }

            static bool VerifySpan(TextSpan span, SyntaxTriviaList leading)
            {
                SyntaxTriviaList.Enumerator en = leading.GetEnumerator();

                while (en.MoveNext())
                {
                    if (!en.Current.IsWhitespaceOrEndOfLineTrivia())
                        continue;

                    if (en.Current.Span.Contains(span.Start))
                    {
                        if (span.IsEmpty)
                            return true;

                        if (en.Current.Span.End == span.End)
                            return true;

                        span = span.TrimFromStart(en.Current.Span.Length);

                        while (en.MoveNext())
                        {
                            if (en.Current.Span.End == span.End)
                                return true;

                            if (span.End < en.Current.Span.End)
                                break;

                            span = span.TrimFromStart(en.Current.Span.Length);
                        }

                        break;
                    }
                }

                return false;
            }
        }
    }
}

