// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.SyntaxRewriters
{
    public sealed class WhitespaceOrEndOfLineRemover : CSharpSyntaxRewriter
    {
        internal static readonly WhitespaceOrEndOfLineRemover Instance = new WhitespaceOrEndOfLineRemover();

        private readonly TextSpan? _span;

        public WhitespaceOrEndOfLineRemover(TextSpan? span = null)
        {
            _span = span;
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.IsWhitespaceOrEndOfLineTrivia()
                && (_span == null || _span.Value.Contains(trivia.Span)))
            {
                return CSharpFactory.EmptyWhitespaceTrivia();
            }

            return base.VisitTrivia(trivia);
        }
    }
}
