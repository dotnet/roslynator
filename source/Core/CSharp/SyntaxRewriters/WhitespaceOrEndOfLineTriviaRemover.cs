// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.SyntaxRewriters
{
    internal class WhitespaceOrEndOfLineTriviaRemover : CSharpSyntaxRewriter
    {
        private static readonly WhitespaceOrEndOfLineTriviaRemover _instance = new WhitespaceOrEndOfLineTriviaRemover();

        private readonly TextSpan? _span;

        private WhitespaceOrEndOfLineTriviaRemover(TextSpan? span = null)
        {
            _span = span;
        }

        public static TNode RemoveWhitespaceOrEndOfLineTrivia<TNode>(TNode node, TextSpan? span = null) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (span == null)
            {
                return (TNode)_instance.Visit(node);
            }
            else
            {
                var remover = new WhitespaceOrEndOfLineTriviaRemover(span);

                return (TNode)remover.Visit(node);
            }
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.IsWhitespaceOrEndOfLineTrivia()
                && (_span == null || _span.Value.Contains(trivia.Span)))
            {
                return CSharpFactory.EmptyWhitespace();
            }
            else
            {
                return base.VisitTrivia(trivia);
            }
        }
    }
}
