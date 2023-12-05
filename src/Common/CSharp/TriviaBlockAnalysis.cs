// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp;

internal readonly struct TriviaBlockAnalysis
{
    private TriviaBlockAnalysis(
        SyntaxNodeOrToken first,
        SyntaxNodeOrToken second,
        TriviaBlockKind kind,
        int position,
        bool singleLineComment = false,
        bool documentationComment = false)
    {
        First = first;
        Second = second;
        Kind = kind;
        Position = position;
        ContainsSingleLineComment = singleLineComment;
        ContainsDocumentationComment = documentationComment;
    }

    public TriviaBlockKind Kind { get; }

    public int Position { get; }

    public SyntaxNodeOrToken First { get; }

    public SyntaxNodeOrToken Second { get; }

    public bool Success => Kind != TriviaBlockKind.Unknown;

    public bool ContainsComment => ContainsSingleLineComment || ContainsDocumentationComment;

    public bool ContainsSingleLineComment { get; }

    public bool ContainsDocumentationComment { get; }

    public bool IsWrapped => Kind == TriviaBlockKind.NewLine || Kind == TriviaBlockKind.BlankLine;

    public Location GetLocation()
    {
        return Location.Create(
            (!First.IsKind(SyntaxKind.None)) ? First.SyntaxTree : Second.SyntaxTree,
            new TextSpan(Position, 0));
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(First, Second);
    }

    public static TriviaBlockAnalysis FromLeading(SyntaxNodeOrToken nodeOrToken)
    {
        var en = new Enumerator(default, nodeOrToken);

        return Analyze(ref en);
    }

    public static TriviaBlockAnalysis FromTrailing(SyntaxNodeOrToken nodeOrToken)
    {
        var en = new Enumerator(nodeOrToken, default);

        return Analyze(ref en);
    }

    public static TriviaBlockAnalysis FromSurrounding(
        SyntaxToken token,
        SyntaxNode nextNode,
        NewLinePosition newLinePosition)
    {
        if (newLinePosition == NewLinePosition.None)
            return default;

        SyntaxToken previousToken = token.GetPreviousToken();

        TriviaBlockAnalysis analysisAfter = FromBetween(token, nextNode);

        if (!analysisAfter.Success)
            return default;

        TriviaBlockAnalysis analysisBefore = FromBetween(previousToken, token);

        if (!analysisBefore.Success)
            return default;

        if (analysisBefore.Kind == TriviaBlockKind.NoNewLine)
        {
            if (analysisAfter.Kind == TriviaBlockKind.NoNewLine)
                return default;

            return (newLinePosition == NewLinePosition.Before)
                ? analysisAfter
                : default;
        }

        if (analysisBefore.ContainsSingleLineComment)
            return default;

        return (newLinePosition == NewLinePosition.Before)
            ? default
            : analysisBefore;
    }

    public static TriviaBlockAnalysis FromBetween(SyntaxNodeOrToken first, SyntaxNodeOrToken second)
    {
        Debug.Assert(first.FullSpan.End == second.FullSpan.Start, $"{first.FullSpan.End} {second.FullSpan.Start}");

        var en = new Enumerator(first, second);

        return Analyze(ref en);
    }

    private static TriviaBlockAnalysis Analyze(ref Enumerator en)
    {
        if (!en.MoveNext())
            return en.GetResult(TriviaBlockKind.NoNewLine);

        if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
            return en.GetResult(TriviaBlockKind.NoNewLine);

        var singleLineComment = false;

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
        {
            singleLineComment = true;

            if (!en.MoveNext())
                return default;
        }

        if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            return en.GetResult(TriviaBlockKind.NoNewLine, singleLineComment, documentationComment: true);

        if (!en.Current.IsEndOfLineTrivia())
            return default;

        if (!en.MoveNext())
            return en.GetResult(TriviaBlockKind.NewLine, singleLineComment);

        if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
            return en.GetResult(TriviaBlockKind.NewLine, singleLineComment);

        if (en.Current.IsDirective)
            return default;

        var singleLineComment2 = false;

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
        {
            singleLineComment = true;
            singleLineComment2 = true;

            if (!en.MoveNext())
                return default;

            if (!en.Current.IsEndOfLineTrivia())
                return default;
        }
        else if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
        {
            return en.GetResult(TriviaBlockKind.NewLine, singleLineComment, documentationComment: true);
        }
        else if (!en.Current.IsEndOfLineTrivia())
        {
            return default;
        }

        TriviaBlockKind kind = (singleLineComment2)
            ? TriviaBlockKind.NewLine
            : TriviaBlockKind.BlankLine;

        while (true)
        {
            if (!en.MoveNext())
                return en.GetResult(kind, singleLineComment);

            if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
                return en.GetResult(kind, singleLineComment);

            if (en.Current.IsDirective)
                return default;

            if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
            {
                singleLineComment = true;
                singleLineComment2 = true;

                if (!en.MoveNext())
                    return default;

                if (!en.Current.IsEndOfLineTrivia())
                    return default;
            }
            else if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            {
                return en.GetResult(kind, singleLineComment, documentationComment: true);
            }
            else if (en.Current.IsEndOfLineTrivia())
            {
                if (singleLineComment2)
                    return default;
            }
            else
            {
                return default;
            }
        }
    }

    public struct Enumerator
    {
        private SyntaxTrivia _previous = default;
        private bool _isSecondTrivia = false;
        private int _firstEolEnd = 0;
        private SyntaxTriviaList.Enumerator _enumerator;

        public Enumerator(SyntaxNodeOrToken first, SyntaxNodeOrToken second)
        {
            Debug.Assert(!first.IsKind(SyntaxKind.None) || !second.IsKind(SyntaxKind.None));

            First = first;
            Second = second;
            Position = first.Span.End;

            _enumerator = (!first.IsKind(SyntaxKind.None))
                ? first.GetTrailingTrivia().GetEnumerator()
                : second.GetLeadingTrivia().GetEnumerator();
        }

        public SyntaxNodeOrToken First { get; }

        public SyntaxNodeOrToken Second { get; }

        public int Position { get; private set; }

        public SyntaxTrivia Current => _enumerator.Current;

        public bool MoveNext()
        {
            if (!_enumerator.MoveNext())
            {
                if (_isSecondTrivia)
                    return false;

                _isSecondTrivia = true;

                if (First.IsKind(SyntaxKind.None)
                    || Second.IsKind(SyntaxKind.None))
                {
                    return false;
                }

                _enumerator = Second.GetLeadingTrivia().GetEnumerator();

                if (!_enumerator.MoveNext())
                    return false;
            }

            if (Current.IsEndOfLineTrivia())
            {
                if (_firstEolEnd == 0)
                {
                    Position = Current.Span.Start;
                    _firstEolEnd = Current.Span.End;
                }
                else if (_firstEolEnd > 0)
                {
                    if (!_previous.IsKind(SyntaxKind.SingleLineCommentTrivia))
                        Position = _firstEolEnd;

                    _firstEolEnd = -1;
                }
            }

            _previous = Current;
            return true;
        }

        internal readonly TriviaBlockAnalysis GetResult(
            TriviaBlockKind kind,
            bool singleLineComment = false,
            bool documentationComment = false)
        {
            return new TriviaBlockAnalysis(First, Second, kind, Position, singleLineComment, documentationComment);
        }
    }
}
