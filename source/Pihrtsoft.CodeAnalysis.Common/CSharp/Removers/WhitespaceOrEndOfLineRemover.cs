// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Removers
{
    public sealed class WhitespaceOrEndOfLineRemover : CSharpSyntaxRewriter
    {
        private static readonly WhitespaceOrEndOfLineRemover _defaultInstance = new WhitespaceOrEndOfLineRemover();

        private readonly TextSpan? _span;

        private WhitespaceOrEndOfLineRemover(TextSpan? span = null)
        {
            _span = span;
        }

        public static TNode RemoveFrom<TNode>(TNode node) where TNode : SyntaxNode
            => (TNode)_defaultInstance.Visit(node);

        public static TNode RemoveFrom<TNode>(TNode node, TextSpan span) where TNode : SyntaxNode
            => (TNode)new WhitespaceOrEndOfLineRemover(span).Visit(node);

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.IsWhitespaceOrEndOfLine()
                && (_span == null || _span.Value.Contains(trivia.Span)))
            {
                return CSharpFactory.EmptyTrivia;
            }

            return base.VisitTrivia(trivia);
        }
    }
}
