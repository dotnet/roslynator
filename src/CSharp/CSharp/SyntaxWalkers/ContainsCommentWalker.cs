// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.SyntaxWalkers
{
    internal sealed class ContainsCommentWalker : CSharpSyntaxWalker
    {
        [ThreadStatic]
        private static ContainsCommentWalker _cachedInstance;

        public ContainsCommentWalker(TextSpan span)
            : base(SyntaxWalkerDepth.Trivia)
        {
            Span = span;
        }

        public bool Result { get; set; }

        public TextSpan Span { get; set; }

        public override void VisitTrivia(SyntaxTrivia trivia)
        {
            if (IsInSpan(trivia.Span)
                && CSharpFacts.IsCommentTrivia(trivia.Kind()))
            {
                Result = true;
            }

            base.VisitTrivia(trivia);
        }

        private bool IsInSpan(TextSpan span)
        {
            return Span.OverlapsWith(span)
                || (span.Length == 0 && Span.IntersectsWith(span));
        }

        public static bool ContainsComment(SyntaxNode node)
        {
            return ContainsComment(node, node.FullSpan);
        }

        public static bool ContainsComment(SyntaxNode node, TextSpan span)
        {
            ContainsCommentWalker walker = GetInstance(span);

            walker.Visit(node);

            bool result = walker.Result;

            Free(walker);

            return result;
        }

        public static ContainsCommentWalker GetInstance(TextSpan span)
        {
            ContainsCommentWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
                walker.Result = false;
                walker.Span = span;

                return walker;
            }

            return new ContainsCommentWalker(span);
        }

        public static void Free(ContainsCommentWalker walker)
        {
            _cachedInstance = walker;
        }
    }
}
