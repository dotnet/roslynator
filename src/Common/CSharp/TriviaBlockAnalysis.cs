// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

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

        internal readonly TriviaBlockAnalysis GetResult(TriviaBlockKind kind, TriviaBlockFlags flags = TriviaBlockFlags.None)
        {
            return new TriviaBlockAnalysis(First, Second, kind, Position, flags);
        }
    }
}
