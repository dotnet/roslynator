// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp;

internal readonly struct TriviaBlockAnalysis
{
    private readonly TriviaBlockFlags _flags;

    private TriviaBlockAnalysis(SyntaxNodeOrToken first, SyntaxNodeOrToken second, TriviaBlockKind kind, int position, TriviaBlockFlags flags)
    {
        First = first;
        Second = second;
        Kind = kind;
        Position = position;
        _flags = flags;
    }

    public TriviaBlockKind Kind { get; }

    public int Position { get; }

    public SyntaxNodeOrToken First { get; }

    public SyntaxNodeOrToken Second { get; }

    public bool Success => Kind != TriviaBlockKind.Unknown;

    public bool ContainsComment => (_flags & (TriviaBlockFlags.Comment)) != 0;

    public bool ContainsSingleLineComment => (_flags & TriviaBlockFlags.SingleLineComment) != 0;

    public bool ContainsDocumentationComment => (_flags & TriviaBlockFlags.DocumentationComment) != 0;

    public Location GetLocation()
    {
        return Location.Create(
            (!First.IsKind(SyntaxKind.None)) ? First.SyntaxTree : Second.SyntaxTree,
            new TextSpan(Position, 0));
    }

    public static TriviaBlockAnalysis AnalyzeBefore(SyntaxNodeOrToken nodeOrToken)
    {
        var en = new Enumerator(default, nodeOrToken);

        return Analyze(ref en);
    }

    public static TriviaBlockAnalysis AnalyzeBetween(SyntaxNodeOrToken first, SyntaxNodeOrToken second)
    {
        Debug.Assert(first.FullSpan.End == second.FullSpan.Start, $"{first.FullSpan.End} {second.FullSpan.Start}");

        var en = new Enumerator(first, second);

        return Analyze(ref en);
    }

    private static TriviaBlockAnalysis Analyze(ref Enumerator en)
    {
        var flags = TriviaBlockFlags.None;

        if (!en.MoveNext())
            return en.GetResult(TriviaBlockKind.NoNewLine, flags);

        if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
            return en.GetResult(TriviaBlockKind.NoNewLine, flags);

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
        {
            flags |= TriviaBlockFlags.SingleLineComment;

            if (!en.MoveNext())
                return default;
        }

        if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            return en.GetResult(TriviaBlockKind.NoNewLine, flags);

        if (!en.Current.IsEndOfLineTrivia())
            return default;

        if (!en.MoveNext())
            return en.GetResult(TriviaBlockKind.NewLine, flags);

        if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
            return en.GetResult(TriviaBlockKind.NewLine, flags);

        if (en.Current.IsDirective)
            return default;

        var singleLineComment = false;

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
        {
            flags |= TriviaBlockFlags.SingleLineComment;
            singleLineComment = true;

            if (!en.MoveNext())
                return default;

            if (!en.Current.IsEndOfLineTrivia())
                return default;
        }
        else if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
        {
            return en.GetResult(TriviaBlockKind.NewLine, flags | TriviaBlockFlags.DocumentationComment);
        }
        else if (!en.Current.IsEndOfLineTrivia())
        {
            return default;
        }

        TriviaBlockKind kind = (singleLineComment)
            ? TriviaBlockKind.NewLine
            : TriviaBlockKind.BlankLine;

        while (true)
        {
            if (!en.MoveNext())
                return en.GetResult(kind, flags);

            if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
                return en.GetResult(kind, flags);

            if (en.Current.IsDirective)
                return default;

            if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
            {
                flags |= TriviaBlockFlags.SingleLineComment;
                singleLineComment = true;

                if (!en.MoveNext())
                    return default;

                if (!en.Current.IsEndOfLineTrivia())
                    return default;
            }
            else if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            {
                return en.GetResult(kind, flags | TriviaBlockFlags.DocumentationComment);
            }
            else if (en.Current.IsEndOfLineTrivia())
            {
                if (singleLineComment)
                    return default;
            }
            else
            {
                return default;
            }
        }
    }

    public static TriviaBlockAnalysis AnalyzeAround(
        SyntaxToken token,
        ExpressionSyntax nextExpression,
        NewLinePosition newLinePosition)
    {
        if (newLinePosition == NewLinePosition.None)
            return default;

        SyntaxToken previousToken = token.GetPreviousToken();

        TriviaBlockAnalysis analysisAfter = AnalyzeBetween(token, nextExpression);

        if (!analysisAfter.Success)
            return default;

        TriviaBlockAnalysis analysisBefore = AnalyzeBetween(previousToken, token);

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

    public Enumerator GetEnumerator()
    {
        return new Enumerator(First, Second);
    }

    public struct Enumerator
    {
        private bool _isSecondTrivia = false;
        private bool _afterFirstEol = false;
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

            if (Current.IsEndOfLineTrivia()
                && !_afterFirstEol)
            {
                Position = Current.Span.End;
                _afterFirstEol = true;
            }

            return true;
        }

        public readonly TriviaBlockAnalysis GetResult(TriviaBlockKind kind, TriviaBlockFlags flags = TriviaBlockFlags.None)
        {
            return new TriviaBlockAnalysis(First, Second, kind, Position, flags);
        }
    }
}
