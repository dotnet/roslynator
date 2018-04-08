// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.SyntaxRewriters
{
    internal class WhitespaceReplacer : CSharpSyntaxRewriter
    {
        public static SyntaxTrivia DefaultReplacement { get; } = CSharpFactory.EmptyWhitespace();

        public SyntaxTrivia Replacement { get; }

        public TextSpan? Span { get; }

        public WhitespaceReplacer(SyntaxTrivia replacement, TextSpan? span = null)
        {
            Replacement = replacement;
            Span = span;
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.IsWhitespaceOrEndOfLineTrivia()
                && (Span == null || Span.Value.Contains(trivia.Span)))
            {
                return Replacement;
            }

            return base.VisitTrivia(trivia);
        }
    }
}
