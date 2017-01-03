// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Extensions
{
    public static class SyntaxNodeExtensions
    {
        internal static bool IsNestedMethod(this SyntaxNode node)
        {
            return node?.IsKind(
                SyntaxKind.SimpleLambdaExpression,
                SyntaxKind.ParenthesizedLambdaExpression,
                SyntaxKind.AnonymousMethodExpression,
                SyntaxKind.LocalFunctionStatement) == true;
        }

        public static IEnumerable<DirectiveTriviaSyntax> DescendantDirectives(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            foreach (SyntaxTrivia trivia in node.DescendantTrivia(descendIntoTrivia: true))
            {
                if (trivia.IsDirective && trivia.HasStructure)
                {
                    var directive = trivia.GetStructure() as DirectiveTriviaSyntax;

                    if (directive != null)
                        yield return directive;
                }
            }
        }

        public static IEnumerable<DirectiveTriviaSyntax> DescendantRegionDirectives(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            foreach (SyntaxNode descendant in node.DescendantNodesAndSelf(descendIntoTrivia: true))
            {
                if (descendant.IsKind(SyntaxKind.RegionDirectiveTrivia, SyntaxKind.EndRegionDirectiveTrivia))
                    yield return (DirectiveTriviaSyntax)descendant;
            }
        }

        public static bool IsDescendantOf(this SyntaxNode node, SyntaxKind kind, bool ascendOutOfTrivia = true)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.Ancestors(ascendOutOfTrivia).Any(f => f.IsKind(kind));
        }

        public static bool IsBooleanLiteralExpression(this SyntaxNode node)
        {
            return node?.IsKind(SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression) == true;
        }

        public static bool IsNumericLiteralExpression(this SyntaxNode node, int value)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.IsKind(SyntaxKind.NumericLiteralExpression))
            {
                object tokenValue = ((LiteralExpressionSyntax)node).Token.Value;

                return tokenValue is int
                    && (int)tokenValue == value;
            }

            return false;
        }

        public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
        {
            if (node == null)
                return false;

            SyntaxKind kind = node.Kind();

            return kind == kind1
                || kind == kind2;
        }

        public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            if (node == null)
                return false;

            SyntaxKind kind = node.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            if (node == null)
                return false;

            SyntaxKind kind = node.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4;
        }

        public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            if (node == null)
                return false;

            SyntaxKind kind = node.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5;
        }

        public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5, SyntaxKind kind6)
        {
            if (node == null)
                return false;

            SyntaxKind kind = node.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5
                || kind == kind6;
        }

        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind)
        {
            return node != null
                && Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node.Parent, kind);
        }

        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
        {
            return node != null
                && IsKind(node.Parent, kind1, kind2);
        }

        public static bool IsParentKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            return node != null
                && IsKind(node.Parent, kind1, kind2, kind3);
        }

        public static SyntaxNode FirstAncestorOf(
            this SyntaxNode node,
            SyntaxKind kind,
            bool ascendOutOfTrivia = true)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node
                .Ancestors(ascendOutOfTrivia)
                .FirstOrDefault(f => f.IsKind(kind));
        }

        public static bool IsSingleLine(
            this SyntaxNode node,
            bool includeExteriorTrivia = true,
            bool trim = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTree syntaxTree = node.SyntaxTree;

            if (syntaxTree != null)
            {
                TextSpan span = GetSpan(node, includeExteriorTrivia, trim);

                return syntaxTree.IsSingleLineSpan(span, cancellationToken);
            }
            else
            {
                return false;
            }
        }

        public static bool IsMultiLine(
            this SyntaxNode node,
            bool includeExteriorTrivia = true,
            bool trim = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTree syntaxTree = node.SyntaxTree;

            if (syntaxTree != null)
            {
                TextSpan span = GetSpan(node, includeExteriorTrivia, trim);

                return syntaxTree.IsMultiLineSpan(span, cancellationToken);
            }
            else
            {
                return false;
            }
        }

        private static TextSpan GetSpan(SyntaxNode node, bool includeExteriorTrivia, bool trim)
        {
            return TextSpan.FromBounds(
                GetStartIndex(node, includeExteriorTrivia, trim),
                GetEndIndex(node, includeExteriorTrivia, trim));
        }

        private static int GetStartIndex(SyntaxNode node, bool includeExteriorTrivia, bool trim)
        {
            if (!includeExteriorTrivia)
                return node.Span.Start;

            int start = node.FullSpan.Start;

            if (trim)
            {
                SyntaxTriviaList leading = node.GetLeadingTrivia();

                for (int i = 0; i < leading.Count; i++)
                {
                    if (!leading[i].IsWhitespaceOrEndOfLineTrivia())
                        break;

                    start = leading[i].Span.End;
                }
            }

            return start;
        }

        private static int GetEndIndex(SyntaxNode node, bool includeExteriorTrivia, bool trim)
        {
            if (!includeExteriorTrivia)
                return node.Span.End;

            int end = node.FullSpan.End;

            if (trim)
            {
                SyntaxTriviaList trailing = node.GetTrailingTrivia();

                for (int i = trailing.Count - 1; i >= 0; i--)
                {
                    if (!trailing[i].IsWhitespaceOrEndOfLineTrivia())
                        break;

                    end = trailing[i].SpanStart;
                }
            }

            return end;
        }

        public static TNode TrimLeadingTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTriviaList leadingTrivia = node.GetLeadingTrivia();

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.TrimStart();

            if (leadingTrivia.Count != newLeadingTrivia.Count)
            {
                return node.WithLeadingTrivia(newLeadingTrivia);
            }
            else
            {
                return node;
            }
        }

        public static TNode TrimTrailingTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTriviaList trailingTrivia = node.GetTrailingTrivia();

            SyntaxTriviaList newTrailingTrivia = trailingTrivia.TrimEnd();

            if (trailingTrivia.Count != newTrailingTrivia.Count)
            {
                return node.WithTrailingTrivia(newTrailingTrivia);
            }
            else
            {
                return node;
            }
        }

        public static TNode TrimTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node
                .TrimLeadingTrivia()
                .TrimTrailingTrivia();
        }

        public static TextSpan TrimmedSpan(
            this SyntaxNode node,
            bool includeExteriorTrivia = true,
            bool trim = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return TextSpan.FromBounds(
                GetStartIndex(node, includeExteriorTrivia: true, trim: true),
                GetEndIndex(node, includeExteriorTrivia: true, trim: true));
        }

        public static TNode WithTrailingSpace<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithTrailingTrivia(Space);
        }
    }
}
