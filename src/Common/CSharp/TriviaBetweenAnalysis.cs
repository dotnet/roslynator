// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp;

internal readonly struct TriviaBetweenAnalysis
{
    private readonly TriviaBetweenFlags _flags;

    private TriviaBetweenAnalysis(SyntaxNodeOrToken first, SyntaxNodeOrToken second, TriviaBetweenKind kind, int position, TriviaBetweenFlags flags)
    {
        First = first;
        Second = second;
        Kind = kind;
        Position = position;
        _flags = flags;
    }

    public TriviaBetweenKind Kind { get; }

    public int Position { get; }

    public SyntaxNodeOrToken First { get; }

    public SyntaxNodeOrToken Second { get; }

    public bool Success => Kind != TriviaBetweenKind.Unknown;

    public bool CanRemoveNewLine => (_flags & TriviaBetweenFlags.SingleLineCommentOnFirstLine) == 0;

    public bool EndsWithDocumentationComment => (_flags & TriviaBetweenFlags.DocumentationComment) != 0;

    public Location GetLocation()
    {
        return Location.Create(First.SyntaxTree, new TextSpan(Position, 0));
    }

    public static TriviaBetweenAnalysis Create(SyntaxNodeOrToken first, SyntaxNodeOrToken second)
    {
        Debug.Assert(first.FullSpan.End == second.FullSpan.Start, $"{first.FullSpan.End} {second.FullSpan.Start}");

        var flags = TriviaBetweenFlags.None;
        var en = new Enumerator(first, second);

        if (!en.MoveNext())
            return en.GetResult(TriviaBetweenKind.NoNewLine, flags);

        if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
            return en.GetResult(TriviaBetweenKind.NoNewLine, flags);

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
        {
            flags |= TriviaBetweenFlags.SingleLineCommentOnFirstLine;

            if (!en.MoveNext())
                return default;
        }

        if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            return en.GetResult(TriviaBetweenKind.NoNewLine, flags);

        if (!en.Current.IsEndOfLineTrivia())
            return default;

        if (!en.MoveNext())
            return en.GetResult(TriviaBetweenKind.NewLine, flags);

        if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
            return en.GetResult(TriviaBetweenKind.NewLine, flags);

        if (en.Current.IsDirective)
            return default;

        var singleLineComment = false;

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
        {
            singleLineComment = true;

            if (!en.MoveNext())
                return default;

            if (!en.Current.IsEndOfLineTrivia())
                return default;
        }
        else if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
        {
            return en.GetResult(TriviaBetweenKind.NewLine, flags | TriviaBetweenFlags.DocumentationComment);
        }
        else if (!en.Current.IsEndOfLineTrivia())
        {
            return default;
        }

        TriviaBetweenKind kind = (singleLineComment)
            ? TriviaBetweenKind.NewLine
            : TriviaBetweenKind.BlankLine;

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
                singleLineComment = true;

                if (!en.MoveNext())
                    return default;

                if (!en.Current.IsEndOfLineTrivia())
                    return default;
            }
            else if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            {
                return en.GetResult(kind, flags | TriviaBetweenFlags.DocumentationComment);
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

            if (Current.IsEndOfLineTrivia()
                && !_afterFirstEol)
            {
                Position = Current.Span.End;
                _afterFirstEol = true;
            }

            return true;
        }

        public readonly TriviaBetweenAnalysis GetResult(TriviaBetweenKind kind, TriviaBetweenFlags flags = TriviaBetweenFlags.None)
        {
            return new TriviaBetweenAnalysis(First, Second, kind, Position, flags);
        }
    }
}
