// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.SyntaxRewriters
{
    internal class TriviaRemover : CSharpSyntaxRewriter
    {
        private TriviaRemover(TextSpan? span = null)
        {
            Span = span;
        }

        private static TriviaRemover Default { get; } = new TriviaRemover();

        public TextSpan? Span { get; }

        public static SyntaxTrivia Replacement { get; } = CSharpFactory.EmptyWhitespace();

        public static TriviaRemover GetInstance(TextSpan? span = null)
        {
            if (span != null)
            {
                return new TriviaRemover(span);
            }
            else
            {
                return Default;
            }
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (Span == null || Span.Value.Contains(trivia.Span))
            {
                return Replacement;
            }

            return base.VisitTrivia(trivia);
        }
    }
}
