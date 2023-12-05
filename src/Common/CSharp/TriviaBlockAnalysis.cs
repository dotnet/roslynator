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
        return Analyze(default, nodeOrToken, nodeOrToken.FullSpan.Start);
    }

    public static TriviaBlockAnalysis FromTrailing(SyntaxNodeOrToken nodeOrToken)
    {
        return Analyze(nodeOrToken, default, nodeOrToken.Span.End);
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

        return Analyze(first, second, first.Span.End);
    }

    private static TriviaBlockAnalysis Analyze(SyntaxNodeOrToken first, SyntaxNodeOrToken second, int position)
    {
        var en = new Enumerator(first, second);
        int firstEolEnd = 0;
        var state = State.Start;
        var commentState = CommentState.BeforeComment;
        var stateBeforeComment = State.Start;

        while (true)
        {
            TriviaLineKind lineKind = en.ReadLine();

            switch (lineKind)
            {
                case TriviaLineKind.EmptyOrWhiteSpace:
                    {
                        if (firstEolEnd == 0)
                        {
                            position = en.Current.Span.Start;
                            firstEolEnd = en.Current.Span.End;
                        }
                        else if (firstEolEnd > 0)
                        {
                            position = firstEolEnd;
                            firstEolEnd = -1;
                        }

                        if (state == State.Start)
                        {
                            state = State.NewLine;
                        }
                        else if (state == State.NewLine)
                        {
                            state = State.Blank;
                        }
                        else if (state == State.Comment)
                        {
                            state = State.Blank;

                            if (commentState == CommentState.Comment)
                                commentState = CommentState.AfterComment;
                        }

                        break;
                    }
                case TriviaLineKind.Comment:
                    {
                        if (first.IsKind(SyntaxKind.None))
                        {
                            position = en.Current.Span.Start;
                        }
                        else if (firstEolEnd == 0)
                        {
                            position = en.Current.Span.Start;
                            firstEolEnd = en.Current.Span.End;
                        }
                        else if (firstEolEnd > 0)
                        {
                            firstEolEnd = -1;
                        }

                        if (state == State.Start)
                        {
                            state = State.NewLine;
                        }
                        else if (state == State.NewLine)
                        {
                            stateBeforeComment = State.NewLine;
                        }
                        else if (state == State.Blank)
                        {
                            stateBeforeComment = State.Blank;

                            if (commentState == CommentState.AfterComment)
                                return default;
                        }

                        commentState = CommentState.Comment;
                        break;
                    }
                case TriviaLineKind.End:
                case TriviaLineKind.DocumentationComment:
                    {
                        return new TriviaBlockAnalysis(
                            first,
                            second,
                            GetKind(state, stateBeforeComment),
                            position,
                            singleLineComment: commentState == CommentState.Comment || commentState == CommentState.AfterComment,
                            documentationComment: lineKind == TriviaLineKind.DocumentationComment);

                        static TriviaBlockKind GetKind(State state, State triviaBeforeComment)
                        {
                            switch (state)
                            {
                                case State.Start:
                                    return TriviaBlockKind.NoNewLine;
                                case State.NewLine:
                                    return TriviaBlockKind.NewLine;
                                case State.Blank:
                                    return TriviaBlockKind.BlankLine;
                                case State.Comment:
                                    switch (triviaBeforeComment)
                                    {
                                        case State.Start:
                                        case State.NewLine:
                                            return TriviaBlockKind.NewLine;
                                        case State.Blank:
                                            return TriviaBlockKind.BlankLine;
                                        default:
                                            throw new InvalidOperationException();
                                    }
                                default:
                                    throw new InvalidOperationException();
                            }
                        }
                    }
                case TriviaLineKind.Unknown:
                    {
                        return default;
                    }
            }
        }
    }

    public struct Enumerator
    {
        private bool _isSecondTrivia = false;
        private SyntaxTriviaList.Enumerator _enumerator;

        public Enumerator(SyntaxNodeOrToken first, SyntaxNodeOrToken second)
        {
            Debug.Assert(!first.IsKind(SyntaxKind.None) || !second.IsKind(SyntaxKind.None));

            First = first;
            Second = second;

            _enumerator = (!first.IsKind(SyntaxKind.None))
                ? first.GetTrailingTrivia().GetEnumerator()
                : second.GetLeadingTrivia().GetEnumerator();
        }

        public SyntaxNodeOrToken First { get; }

        public SyntaxNodeOrToken Second { get; }

        public SyntaxTrivia Current => _enumerator.Current;

        public TriviaLineKind ReadLine()
        {
            while (MoveNext())
            {
                switch (Current.Kind())
                {
                    case SyntaxKind.WhitespaceTrivia:
                        break;
                    case SyntaxKind.EndOfLineTrivia:
                        return TriviaLineKind.EmptyOrWhiteSpace;
                    case SyntaxKind.MultiLineCommentTrivia:
                        return TriviaLineKind.Unknown;
                    case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    case SyntaxKind.MultiLineDocumentationCommentTrivia:
                        return TriviaLineKind.DocumentationComment;
                    case SyntaxKind.SingleLineCommentTrivia:
                        {
                            if (MoveNext()
                                && Current.IsEndOfLineTrivia())
                            {
                                return TriviaLineKind.Comment;
                            }

                            return TriviaLineKind.Unknown;
                        }
                }

                if (Current.IsDirective)
                    return TriviaLineKind.Unknown;
            }

            return TriviaLineKind.End;
        }

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

            return true;
        }
    }

    internal enum TriviaLineKind
    {
        EmptyOrWhiteSpace,
        Comment,
        End,
        DocumentationComment,
        Unknown,
    }

    private enum State
    {
        Start,
        NewLine,
        Blank,
        Comment,
    }

    private enum CommentState
    {
        BeforeComment,
        Comment,
        AfterComment,
    }
}
