// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp;

internal struct TriviaBlockReader
{
    private bool _isSecondTrivia = false;
    private int _index = -1;
    private SyntaxTriviaList _list;
    private readonly SyntaxNodeOrToken _second;

    internal TriviaBlockReader(SyntaxNodeOrToken first, SyntaxNodeOrToken second)
    {
        Debug.Assert(!first.IsKind(SyntaxKind.None) || !second.IsKind(SyntaxKind.None));

        _second = second;

        if (!first.IsKind(SyntaxKind.None))
        {
            _list = first.GetTrailingTrivia();
        }
        else
        {
            _list = second.GetLeadingTrivia();
            _isSecondTrivia = true;
        }
    }

    public readonly SyntaxTrivia Current => _list[_index];

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

    public void ReadTo(int position)
    {
        while (MoveNext()
            && Current.SpanStart != position)
        {
        }
    }

    public bool ReadWhiteSpaceTrivia()
    {
        if (Peek().IsWhitespaceTrivia())
        {
            MoveNext();
            return true;
        }

        return false;
    }

    public void ReadWhiteSpace()
    {
        while (Peek().IsWhitespaceOrEndOfLineTrivia())
            MoveNext();
    }

    public void ReadBlankLines()
    {
        while (true)
        {
            if (Peek().IsWhitespaceTrivia())
            {
                if (Peek(1).IsEndOfLineTrivia())
                {
                    MoveNext();
                    MoveNext();
                }
                else
                {
                    break;
                }
            }
            else if (Peek().IsEndOfLineTrivia())
            {
                MoveNext();
            }
            else
            {
                break;
            }
        }
    }

    public readonly SyntaxTrivia Peek(int offset = 0)
    {
        offset++;
        if (_index + offset < _list.Count)
            return _list[_index + offset];

        if (!_isSecondTrivia)
        {
            SyntaxTriviaList list = _second.GetLeadingTrivia();

            int index = offset - (_list.Count - _index);

            if (index < list.Count)
                return list[index];
        }

        return default;
    }

    private bool MoveNext()
    {
        if (_index < _list.Count - 1)
        {
            _index++;
            return true;
        }

        if (!_isSecondTrivia)
        {
            _list = _second.GetLeadingTrivia();
            _index = -1;
            _isSecondTrivia = true;

            if (_index < _list.Count - 1)
            {
                _index++;
                return true;
            }
        }

        return false;
    }
}
