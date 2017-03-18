// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.Extensions
{
    public static class SyntaxNodeExtensions
    {
        public static IEnumerable<SyntaxTrivia> GetLeadingAndTrailingTrivia(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.GetLeadingTrivia().Concat(node.GetTrailingTrivia());
        }

        public static TNode PrependToLeadingTrivia<TNode>(this TNode node, IEnumerable<SyntaxTrivia> trivia) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (trivia == null)
                throw new ArgumentNullException(nameof(trivia));

            return node.WithLeadingTrivia(trivia.Concat(node.GetLeadingTrivia()));
        }

        public static TNode PrependToLeadingTrivia<TNode>(this TNode node, SyntaxTrivia trivia) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithLeadingTrivia(node.GetLeadingTrivia().Insert(0, trivia));
        }

        public static TNode AppendToTrailingTrivia<TNode>(this TNode node, IEnumerable<SyntaxTrivia> trivia) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (trivia == null)
                throw new ArgumentNullException(nameof(trivia));

            return node.WithTrailingTrivia(node.GetTrailingTrivia().AddRange(trivia));
        }

        public static TNode AppendToTrailingTrivia<TNode>(this TNode node, SyntaxTrivia trivia) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithTrailingTrivia(node.GetTrailingTrivia().Add(trivia));
        }

        public static bool SpanContainsDirectives(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.ContainsDirectives
                && !node.GetLeadingTrivia().Any(f => f.IsDirective)
                && !node.GetTrailingTrivia().Any(f => f.IsDirective);
        }

        public static bool ContainsDirectives(this SyntaxNode node, TextSpan span)
        {
            return node.ContainsDirectives
                && node.DescendantTrivia(span).Any(f => f.IsDirective);
        }

        public static TNode WithTriviaFrom<TNode>(this TNode syntax, SyntaxToken token) where TNode : SyntaxNode
        {
            if (syntax == null)
                throw new ArgumentNullException(nameof(syntax));

            return syntax
                .WithLeadingTrivia(token.LeadingTrivia)
                .WithTrailingTrivia(token.TrailingTrivia);
        }

        public static int GetSpanStartLine(this SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.SyntaxTree != null)
            {
                return node.SyntaxTree.GetLineSpan(node.Span, cancellationToken).StartLine();
            }
            else
            {
                return -1;
            }
        }

        public static int GetFullSpanStartLine(
            this SyntaxNode node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.SyntaxTree != null)
            {
                return node.SyntaxTree.GetLineSpan(node.FullSpan, cancellationToken).StartLine();
            }
            else
            {
                return -1;
            }
        }

        public static int GetSpanEndLine(this SyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.SyntaxTree != null)
            {
                return node.SyntaxTree.GetLineSpan(node.Span, cancellationToken).EndLine();
            }
            else
            {
                return -1;
            }
        }

        public static int GetFullSpanEndLine(
            this SyntaxNode node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.SyntaxTree != null)
            {
                return node.SyntaxTree.GetLineSpan(node.FullSpan, cancellationToken).EndLine();
            }
            else
            {
                return -1;
            }
        }

        public static TNode FirstAncestor<TNode>(
            this SyntaxNode node,
            Func<TNode, bool> predicate = null,
            bool ascendOutOfTrivia = true) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.Parent?.FirstAncestorOrSelf(predicate, ascendOutOfTrivia);
        }

        public static TNode WithFormatterAnnotation<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithAdditionalAnnotations(Formatter.Annotation);
        }

        public static TNode WithSimplifierAnnotation<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithAdditionalAnnotations(Simplifier.Annotation);
        }

        internal static string ToString(this SyntaxNode node, TextSpan span)
        {
            return GetSubstring(node, node.ToString(), span);
        }

        internal static string ToFullString(this SyntaxNode node, TextSpan span)
        {
            return GetSubstring(node, node.ToFullString(), span);
        }

        private static string GetSubstring(SyntaxNode node, string s, TextSpan span)
        {
            return s.Substring(span.Start - node.SpanStart, span.Length);
        }
    }
}
