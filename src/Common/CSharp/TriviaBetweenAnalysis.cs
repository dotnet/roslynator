// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp;

internal readonly struct TriviaBetweenAnalysis
{
    private TriviaBetweenAnalysis(SyntaxNodeOrToken first, SyntaxNodeOrToken second, TriviaBetweenKind kind, int position)
    {
        First = first;
        Second = second;
        Kind = kind;
        Position = position;
    }

    public TriviaBetweenKind Kind { get; }

    public int Position { get; }

    public SyntaxNodeOrToken First { get; }

    public SyntaxNodeOrToken Second { get; }

    public static TriviaBetweenAnalysis Create(SyntaxNodeOrToken first, SyntaxNodeOrToken second)
    {
        Debug.Assert(first.FullSpan.End == second.FullSpan.Start, $"{first.FullSpan.End} {second.FullSpan.Start}");

        var en = new Enumerator(first, second);

        if (!en.MoveNext())
            return en.GetResult(TriviaBetweenKind.NoNewLine);

        if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
            return en.GetResult(TriviaBetweenKind.NoNewLine);

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia) && !en.MoveNext())
            return default;

        if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            return en.GetResult(TriviaBetweenKind.NoNewLine);

        if (!en.Current.IsEndOfLineTrivia())
            return default;

        if (!en.MoveNext())
            return en.GetResult(TriviaBetweenKind.NewLine);

        if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
            return en.GetResult(TriviaBetweenKind.NewLine);

        if (en.Current.IsDirective)
            return default;

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
            return default;

        if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            return en.GetResult(TriviaBetweenKind.NewLine);

        if (!en.Current.IsEndOfLineTrivia())
            return default;

        if (!en.MoveNext())
            return en.GetResult(TriviaBetweenKind.BlankLine);

        if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
            return en.GetResult(TriviaBetweenKind.BlankLine);

        if (en.Current.IsDirective)
            return default;

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
            return default;

        if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            return en.GetResult(TriviaBetweenKind.BlankLine);

        if (!en.Current.IsEndOfLineTrivia())
            return default;

        return en.GetResult(TriviaBetweenKind.BlankLines);
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(First, Second);
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

        string endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(first).ToString();
        TriviaBetweenAnalysis analysis = Create(first, second);

        Debug.Assert(position == analysis.Position);

        switch (analysis.Kind)
        {
            case TriviaBetweenKind.NoNewLine:
                {
                    Enumerator en = analysis.GetEnumerator();
                    string newText = endOfLine;

                    if (!en.MoveNext())
                    {
                        newText += endOfLine;
                    }
                    else if (en.Current.IsWhitespaceTrivia())
                    {
                        position = en.Current.Span.End;
                    }

                    return new TextChange(new TextSpan(position, 0), newText);
                }
            case TriviaBetweenKind.NewLine:
                {
                    Enumerator en = analysis.GetEnumerator();

                    while (en.MoveNext()
                        && !en.Current.IsEndOfLineTrivia())
                    {
                    }

                    return new TextChange(new TextSpan(en.Current.Span.End, 0), endOfLine);
                }
            case TriviaBetweenKind.BlankLine:
            case TriviaBetweenKind.BlankLines:
                {
                    Enumerator en = analysis.GetEnumerator();

                    while (en.MoveNext()
                        && en.Current.SpanStart != position)
                    {
                    }

                    int end = en.Current.Span.End;

                    while (en.MoveNext())
                    {
                        if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
                            break;

                        if (!en.Current.IsEndOfLineTrivia())
                            break;

                        end = en.Current.Span.End;
                    }

                    return new TextChange(TextSpan.FromBounds(position, end), "");
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
            First = first;
            Second = second;
            Trailing = first.GetTrailingTrivia();
            Leading = second.GetLeadingTrivia();
            Position = first.Span.End;

            _enumerator = Trailing.GetEnumerator();
        }

        public SyntaxNodeOrToken First { get; }

        public SyntaxNodeOrToken Second { get; }

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
            return new TriviaBetweenAnalysis(First, Second, kind, Position);
        }
    }
}
