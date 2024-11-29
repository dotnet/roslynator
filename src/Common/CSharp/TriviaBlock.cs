// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp;

internal readonly struct TriviaBlock
{
    private TriviaBlock(
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

    internal int Position { get; }

    public SyntaxNodeOrToken First { get; }

    public SyntaxNodeOrToken Second { get; }

    public SyntaxNodeOrToken FirstOrSecond => (!First.IsKind(SyntaxKind.None)) ? First : Second;

    public bool Success => Kind != TriviaBlockKind.Unknown;

    public bool ContainsComment => ContainsSingleLineComment || ContainsDocumentationComment;

    public bool ContainsSingleLineComment { get; }

    public bool ContainsDocumentationComment { get; }

    public bool IsWrapped => Kind == TriviaBlockKind.NewLine || Kind == TriviaBlockKind.BlankLine;

    public Location GetLocation()
    {
        SyntaxTriviaList triviaList = (!First.IsKind(SyntaxKind.None))
            ? First.GetTrailingTrivia()
            : Second.GetLeadingTrivia();

        TextSpan span = triviaList.FirstOrDefault(f => f.IsWhitespaceOrEndOfLineTrivia()).Span;

        if (span.Length == 0)
        {
            if (Second.Span.Length > 0)
            {
                span = new TextSpan(Second.Span.Start, 1);
            }
            else if (First.Span.Length > 0)
            {
                span = new TextSpan(First.Span.End, 1);
            }
            else
            {
                span = new TextSpan(Position, 0);
            }
        }

        return Location.Create(
            (!First.IsKind(SyntaxKind.None)) ? First.SyntaxTree : Second.SyntaxTree,
            span);
    }

    public TriviaBlockReader CreateReader()
    {
        return new(First, Second);
    }

    public static TriviaBlock FromLeading(SyntaxNodeOrToken nodeOrToken)
    {
        return Analyze(default, nodeOrToken, nodeOrToken.FullSpan.Start);
    }

    public static TriviaBlock FromTrailing(SyntaxNodeOrToken nodeOrToken)
    {
        return Analyze(nodeOrToken, default, nodeOrToken.Span.End);
    }

    public static TriviaBlock FromSurrounding(
        SyntaxToken token,
        SyntaxNode nextNode,
        NewLinePosition newLinePosition)
    {
        if (newLinePosition == NewLinePosition.None)
            return default;

        SyntaxToken previousToken = token.GetPreviousToken();

        TriviaBlock after = FromBetween(token, nextNode);

        if (!after.Success)
            return default;

        TriviaBlock before = FromBetween(previousToken, token);

        if (!before.Success)
            return default;

        if (before.Kind == TriviaBlockKind.NoNewLine)
        {
            if (after.Kind == TriviaBlockKind.NoNewLine)
                return default;

            return (newLinePosition == NewLinePosition.Before)
                ? after
                : default;
        }

        if (before.ContainsSingleLineComment)
            return default;

        return (newLinePosition == NewLinePosition.Before)
            ? default
            : before;
    }

    public static TriviaBlock FromBetween(SyntaxNodeOrToken first, SyntaxNodeOrToken second)
    {
        Debug.Assert(first.FullSpan.End == second.FullSpan.Start, $"{first.FullSpan.End} {second.FullSpan.Start}");

        return Analyze(first, second, first.Span.End);
    }

    private static TriviaBlock Analyze(SyntaxNodeOrToken first, SyntaxNodeOrToken second, int position)
    {
        var reader = new TriviaBlockReader(first, second);

        int firstEolEnd = -1;
        var state = State.Start;
        var commentState = CommentState.BeforeComment;
        var stateBeforeComment = State.Start;

        while (true)
        {
            SyntaxTrivia trivia = reader.ReadLine();

            if (trivia.IsDirective)
                return default;

            switch (trivia.Kind())
            {
                case SyntaxKind.MultiLineCommentTrivia:
                {
                    return default;
                }
                case SyntaxKind.EndOfLineTrivia:
                {
                    if (firstEolEnd == -1)
                    {
                        position = reader.Current.Span.Start;
                        firstEolEnd = reader.Current.Span.End;
                    }
                    else if (firstEolEnd >= 0)
                    {
                        position = firstEolEnd;
                        firstEolEnd = -2;
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
                case SyntaxKind.SingleLineCommentTrivia:
                {
                    if (!reader.Read(SyntaxKind.EndOfLineTrivia))
                        return default;

                    if (first.IsKind(SyntaxKind.None))
                    {
                        position = reader.Current.Span.Start;
                        firstEolEnd = reader.Current.Span.End;
                    }
                    else if (firstEolEnd == -1)
                    {
                        position = reader.Current.Span.Start;
                        firstEolEnd = reader.Current.Span.End;
                    }
                    else if (firstEolEnd >= 0)
                    {
                        firstEolEnd = -2;
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
                case SyntaxKind.None:
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                {
                    return new TriviaBlock(
                        first,
                        second,
                        GetKind(state, stateBeforeComment),
                        position,
                        singleLineComment: commentState == CommentState.Comment || commentState == CommentState.AfterComment,
                        documentationComment: SyntaxFacts.IsDocumentationCommentTrivia(trivia.Kind()));

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
                default:
                {
                    Debug.Fail(trivia.Kind().ToString());
                    return default;
                }
            }
        }
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
