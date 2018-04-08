// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.SyntaxRewriters
{
    internal class WhitespaceRemover : CSharpSyntaxRewriter
    {
        private WhitespaceRemover(TextSpan? span = null)
        {
            Span = span;
        }

        private static WhitespaceRemover Default { get; } = new WhitespaceRemover();

        public TextSpan? Span { get; }

        public static SyntaxTrivia Replacement { get; } = CSharpFactory.EmptyWhitespace();

        public static WhitespaceRemover GetInstance(TextSpan? span = null)
        {
            if (span != null)
            {
                return new WhitespaceRemover(span);
            }
            else
            {
                return Default;
            }
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
