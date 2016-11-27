// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.SyntaxRewriters
{
    internal class TriviaSyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly TextSpan _textSpan;

        public TriviaSyntaxRewriter(TextSpan textSpan)
        {
            _textSpan = textSpan;
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (_textSpan.Contains(trivia.Span))
            {
                return CSharpFactory.EmptyWhitespaceTrivia();
            }
            else
            {
                return base.VisitTrivia(trivia);
            }
        }
    }
}
