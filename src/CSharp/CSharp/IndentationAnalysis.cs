// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct IndentationAnalysis
    {
        private IndentationAnalysis(SyntaxTrivia indentation, int indentSize)
        {
            Indentation = indentation;
            IndentSize = indentSize;
        }

        public SyntaxTrivia Indentation { get; }

        public int IndentSize { get; }

        public int IndentationLength => Indentation.Span.Length;

        public int IncreasedIndentationLength => IndentationLength + IndentSize;

        public bool IsDefault => Indentation == default;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"\"{Indentation}\" Length = {Indentation.Span.Length} {nameof(IndentSize)} = {IndentSize}";

        public static IndentationAnalysis Create(SyntaxNode node, CancellationToken cancellationToken = default)
        {
            SyntaxTrivia indentation = SyntaxTriviaAnalysis.DetermineIndentation(node, cancellationToken);

            int size = SyntaxTriviaAnalysis.DetermineIndentationSize(node, cancellationToken);

            return new IndentationAnalysis(indentation, size);
        }

        public string GetIncreasedIndentation()
        {
            string indentation = Indentation.ToString();

            string singleIndentation = GetSingleIndentation(indentation);

            return indentation + singleIndentation;
        }

        public SyntaxTrivia GetIncreasedIndentationTrivia()
        {
            return SyntaxFactory.Whitespace(GetIncreasedIndentation());
        }

        public SyntaxTriviaList GetIncreasedIndentationTriviaList()
        {
            return SyntaxFactory.TriviaList(GetIncreasedIndentationTrivia());
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
    }
}
