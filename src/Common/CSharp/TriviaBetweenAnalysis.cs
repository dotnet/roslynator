// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp;

internal readonly struct TriviaBetweenAnalysis
{
    private TriviaBetweenAnalysis(TriviaBetweenKind kind, int position)
    {
        Kind = kind;
        Position = position;
    }

    public TriviaBetweenKind Kind { get; }

    public int Position { get; }

    public static TriviaBetweenAnalysis Create(SyntaxNodeOrToken first, SyntaxNodeOrToken second)
    {
        Debug.Assert(first.FullSpan.End == second.FullSpan.Start, $"{first.FullSpan.End} {first.FullSpan.Start}");

        var en = new Enumerator(first, second);

        if (!en.MoveNext())
            return en.GetResult(TriviaBetweenKind.NoNewLine);

        if (en.Current.IsWhitespaceTrivia()
            && !en.MoveNext())
        {
            return en.GetResult(TriviaBetweenKind.NoNewLine);
        }

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia)
            && !en.MoveNext())
        {
            return en.GetResult(TriviaBetweenKind.Unknown);
        }

        if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            return en.GetResult(TriviaBetweenKind.NoNewLine);

        if (!en.Current.IsEndOfLineTrivia())
            return en.GetResult(TriviaBetweenKind.Unknown);

        if (!en.MoveNext())
            return en.GetResult(TriviaBetweenKind.NewLine);

        if (en.Current.IsWhitespaceTrivia()
            && !en.MoveNext())
        {
            return en.GetResult(TriviaBetweenKind.NewLine);
        }

        if (en.Current.IsDirective)
            return en.GetResult(TriviaBetweenKind.PreprocessorDirective);

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
            return en.GetResult(TriviaBetweenKind.Unknown);

        if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            return en.GetResult(TriviaBetweenKind.NewLine);

        if (!en.Current.IsEndOfLineTrivia())
            return en.GetResult(TriviaBetweenKind.Unknown);

        if (!en.MoveNext())
            return en.GetResult(TriviaBetweenKind.BlankLine);

        if (en.Current.IsWhitespaceTrivia()
            && !en.MoveNext())
        {
            return en.GetResult(TriviaBetweenKind.BlankLine);
        }

        if (en.Current.IsDirective)
            return en.GetResult(TriviaBetweenKind.PreprocessorDirective);

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
            return en.GetResult(TriviaBetweenKind.Unknown);

        if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            return en.GetResult(TriviaBetweenKind.BlankLine);

        if (!en.Current.IsEndOfLineTrivia())
            return en.GetResult(TriviaBetweenKind.Unknown);

        return en.GetResult(TriviaBetweenKind.BlankLines);
    }

    public static TextChange GetTextChangeForBlankLine(SyntaxNode root, int position)
    {
        SyntaxToken token = root.FindToken(position);

        SyntaxToken first;
        SyntaxToken second;

        if (token.Span.End <= position)
        {
            first = token;
            second = token.GetNextToken();
        }
        else
        {
            second = token;
            first = token.GetPreviousToken();
        }

        string endofLine = SyntaxTriviaAnalysis.DetermineEndOfLine(first).ToString();
        TriviaBetweenAnalysis analysis = Create(first, second);

        Debug.Assert(position == analysis.Position);

        switch (analysis.Kind)
        {
            case TriviaBetweenKind.NoNewLine:
            case TriviaBetweenKind.NewLine:
                {
                    SyntaxTrivia trivia = root.FindTrivia(position);

                    if (trivia.IsEndOfLineTrivia())
                        return new TextChange(new TextSpan(trivia.Span.End, 0), endofLine);

                    if (!trivia.IsKind(SyntaxKind.None))
                    {
                        SyntaxTriviaList trailing = first.TrailingTrivia;
                        int count = trailing.Count;
                        int index = trailing.IndexOf(trivia);

                        if (index >= 0)
                        {
                            if (index < count - 1
                                && trailing[index + 1].IsWhitespaceTrivia())
                            {
                                index++;
                                position = trailing[index].Span.End;
                            }

                            if (index < count - 1
                                && trailing[index + 1].IsKind(SyntaxKind.SingleLineCommentTrivia))
                            {
                                index++;
                                position = trailing[index].Span.End;
                            }

                            if (index < count - 1
                                && trailing[index + 1].IsEndOfLineTrivia())
                            {
                                return new TextChange(new TextSpan(trailing[index + 1].Span.End, 0), endofLine);
                            }
                        }
                    }

                    return new TextChange(new TextSpan(position, 0), endofLine + endofLine);
                }
            case TriviaBetweenKind.BlankLine:
            case TriviaBetweenKind.BlankLines:
                {
                    SyntaxTrivia trivia = root.FindTrivia(position);
                    int start = trivia.SpanStart;
                    int end = trivia.Span.End;
                    SyntaxTriviaList list = trivia.GetContainingList();
                    int index = list.IndexOf(trivia);

                    IEnumerator<SyntaxTrivia> en = list.Skip(index + 1).GetEnumerator();

                    while (en.MoveNext())
                    {
                        if (en.Current.IsWhitespaceTrivia()
                            && !en.MoveNext())
                        {
                            break;
                        }

                        if (en.Current.IsEndOfLineTrivia())
                        {
                            end = en.Current.Span.End;
                        }
                        else
                        {
                            break;
                        }
                    }

                    return new TextChange(TextSpan.FromBounds(start, end), "");
                }
            default:
                {
                    throw new InvalidOperationException();
                }
        }
    }

    public struct Enumerator
    {
        private bool _isSecondTrivia = false;
        private int _eolState = 0;
        private SyntaxTriviaList.Enumerator _enumerator;

        public Enumerator(SyntaxNodeOrToken first, SyntaxNodeOrToken second)
        {
            Trailing = first.GetTrailingTrivia();
            Leading = second.GetLeadingTrivia();
            Position = first.Span.End;

            _enumerator = Trailing.GetEnumerator();
        }

        public SyntaxTriviaList Trailing { get; set; }

        public SyntaxTriviaList Leading { get; set; }

        public int Position { get; private set; }

        public SyntaxTrivia Current => _enumerator.Current;

        public bool MoveNext()
        {
            if (!_enumerator.MoveNext())
            {
                if (_isSecondTrivia)
                    return false;

                _enumerator = Leading.GetEnumerator();
                _isSecondTrivia = true;

                if (!_enumerator.MoveNext())
                    return false;
            }

            if (Current.IsEndOfLineTrivia())
            {
                if (_eolState == 0)
                {
                    _eolState = 1;
                }
                else if (_eolState == 1)
                {
                    Position = Current.SpanStart;
                    _eolState = 2;
                }
            }

            return true;
        }

        public readonly TriviaBetweenAnalysis GetResult(TriviaBetweenKind kind)
        {
            return new TriviaBetweenAnalysis(kind, Position);
        }
    }
}
