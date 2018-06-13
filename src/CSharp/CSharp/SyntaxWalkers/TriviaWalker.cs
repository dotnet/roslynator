// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.SyntaxWalkers
{
    internal abstract class TriviaWalker : CSharpSyntaxWalker
    {
        public TextSpan Span { get; protected set; }

        protected TriviaWalker(TextSpan span) : base(SyntaxWalkerDepth.Trivia)
        {
            Span = span;
        }

        public static bool ContainsOnlyWhitespaceOrEndOfLineTrivia(SyntaxNode node, TextSpan? span = null)
        {
            ContainsOnlyWhitespaceOrEndOfLineTriviaWalker walker = ContainsOnlyWhitespaceOrEndOfLineTriviaWalker.GetInstance(span ?? node.FullSpan);

            walker.Visit(node);

            bool result = walker.Result;

            ContainsOnlyWhitespaceOrEndOfLineTriviaWalker.Free(walker);

            return result;
        }

        public override void Visit(SyntaxNode node)
        {
            if (IsInSpan(node.FullSpan))
                base.Visit(node);
        }

        private bool IsInSpan(TextSpan span)
        {
            return Span.OverlapsWith(span)
                || (span.Length == 0 && Span.IntersectsWith(span));
        }

        public override void VisitLeadingTrivia(SyntaxToken token)
        {
            if (IsInSpan(token.FullSpan))
            {
                SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

                if (leadingTrivia.Count > 0)
                {
                    foreach (SyntaxTrivia trivia in leadingTrivia)
                        VisitTrivia(trivia);
                }
            }
        }

        public override void VisitTrailingTrivia(SyntaxToken token)
        {
            if (IsInSpan(token.FullSpan))
            {
                SyntaxTriviaList trailingTrivia = token.TrailingTrivia;

                if (trailingTrivia.Count > 0)
                {
                    foreach (SyntaxTrivia trivia in trailingTrivia)
                        VisitTrivia(trivia);
                }
            }
        }

        public override void VisitTrivia(SyntaxTrivia trivia)
        {
            if (IsInSpan(trivia.FullSpan))
                VisitTriviaCore(trivia);
        }

        protected abstract void VisitTriviaCore(SyntaxTrivia trivia);

        private sealed class ContainsOnlyWhitespaceOrEndOfLineTriviaWalker : TriviaWalker
        {
            [ThreadStatic]
            private static ContainsOnlyWhitespaceOrEndOfLineTriviaWalker _cachedInstance;

            public bool Result { get; private set; } = true;

            public ContainsOnlyWhitespaceOrEndOfLineTriviaWalker(TextSpan span) : base(span)
            {
            }

            public override void Visit(SyntaxNode node)
            {
                if (Result)
                    base.Visit(node);
            }

            protected override void VisitTriviaCore(SyntaxTrivia trivia)
            {
                if (!trivia.IsWhitespaceOrEndOfLineTrivia())
                    Result = false;
            }

            public static ContainsOnlyWhitespaceOrEndOfLineTriviaWalker GetInstance(TextSpan span)
            {
                ContainsOnlyWhitespaceOrEndOfLineTriviaWalker walker = _cachedInstance;

                if (walker != null)
                {
                    _cachedInstance = null;
                    walker.Result = true;
                    walker.Span = span;
                    return walker;
                }

                return new ContainsOnlyWhitespaceOrEndOfLineTriviaWalker(span);
            }

            public static void Free(ContainsOnlyWhitespaceOrEndOfLineTriviaWalker walker)
            {
                _cachedInstance = walker;
            }
        }
    }
}
