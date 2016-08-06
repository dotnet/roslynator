// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Removers
{
    public sealed class WhitespaceOrEndOfLineRemover : CSharpSyntaxRewriter
    {
        private static readonly WhitespaceOrEndOfLineRemover _instance = new WhitespaceOrEndOfLineRemover();

        private readonly TextSpan? _span;

        private WhitespaceOrEndOfLineRemover(TextSpan? span = null)
        {
            _span = span;
        }

        public static TNode RemoveFrom<TNode>(TNode node) where TNode : SyntaxNode
        {
            return (TNode)_instance.Visit(node);
        }

        public static TNode RemoveFrom<TNode>(TNode node, TextSpan span) where TNode : SyntaxNode
        {
            return (TNode)new WhitespaceOrEndOfLineRemover(span).Visit(node);
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.IsWhitespaceOrEndOfLineTrivia()
                && (_span == null || _span.Value.Contains(trivia.Span)))
            {
                return CSharpFactory.EmptyTrivia;
            }

            return base.VisitTrivia(trivia);
        }
    }
}
