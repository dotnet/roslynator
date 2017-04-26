// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.SyntaxRewriters
{
    internal class WhitespaceOrEndOfLineTriviaRewriter : CSharpSyntaxRewriter
    {
        private readonly SyntaxTrivia _replacementTrivia;
        private readonly TextSpan? _span;

        private static readonly SyntaxTrivia _defaultReplacementTrivia = CSharpFactory.EmptyWhitespace();

        public static WhitespaceOrEndOfLineTriviaRewriter Default { get; } = new WhitespaceOrEndOfLineTriviaRewriter(_defaultReplacementTrivia);

        private WhitespaceOrEndOfLineTriviaRewriter(SyntaxTrivia replacementTrivia, TextSpan? span = null)
        {
            _replacementTrivia = replacementTrivia;
            _span = span;
        }

        public static TNode RemoveWhitespaceOrEndOfLineTrivia<TNode>(TNode node, TextSpan? span = null) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (span == null)
            {
                return (TNode)Default.Visit(node);
            }
            else
            {
                var remover = new WhitespaceOrEndOfLineTriviaRewriter(_defaultReplacementTrivia, span);

                return (TNode)remover.Visit(node);
            }
        }

        public static TNode ReplaceWhitespaceOrEndOfLineTrivia<TNode>(TNode node, SyntaxTrivia replacementTrivia, TextSpan? span = null) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var rewriter = new WhitespaceOrEndOfLineTriviaRewriter(replacementTrivia, span);

            return (TNode)rewriter.Visit(node);
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.IsWhitespaceOrEndOfLineTrivia()
                && (_span == null || _span.Value.Contains(trivia.Span)))
            {
                return _replacementTrivia;
            }
            else
            {
                return base.VisitTrivia(trivia);
            }
        }
    }
}
