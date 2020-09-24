// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct IndentationAnalysis
    {
        public static IndentationAnalysis Empty { get; } = new IndentationAnalysis(CSharpFactory.EmptyWhitespace(), 0);

        internal IndentationAnalysis(SyntaxTrivia indentation, int indentSize)
        {
            Indentation = indentation;
            IndentSize = indentSize;
        }

        public SyntaxTrivia Indentation { get; }

        public int IndentSize { get; }

        public bool IsEmpty => Indentation.Span.Length == 0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"\"{Indentation}\" Length = {Indentation.Span.Length} {nameof(IndentSize)} = {IndentSize}";

        public string GetIncreasedIndentation()
        {
            string indentation = Indentation.ToString();

            string singleIndentation = GetSingleIndentation(indentation);

            return indentation + singleIndentation;
        }

        public string GetSingleIndentation()
        {
            return GetSingleIndentation(Indentation.ToString());
        }

        private string GetSingleIndentation(string indentation)
        {
            if (indentation.Length == 0)
                return "";

            if (indentation[indentation.Length - 1] == '\t')
                return "\t";

            return new string(indentation[0], IndentSize);
        }

        public SyntaxTrivia GetIncreasedIndentationTrivia()
        {
            return SyntaxFactory.Whitespace(GetIncreasedIndentation());
        }

        public SyntaxTriviaList GetIncreasedIndentationTriviaList()
        {
            return SyntaxFactory.TriviaList(GetIncreasedIndentationTrivia());
        }
    }
}
