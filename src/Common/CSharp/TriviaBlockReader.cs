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

    public SyntaxTrivia ReadLine()
    {
        while (MoveNext())
        {
            if (!Current.IsWhitespaceTrivia())
                return Current;
        }

        return default;
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
        return Read(SyntaxKind.WhitespaceTrivia);
    }

    public bool Read(SyntaxKind kind)
    {
        if (Peek().IsKind(kind))
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
