// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CodeFixes
{
    public abstract class AbstractCodeFixProvider : CodeFixProvider
    {
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        protected virtual string GetEquivalenceKey(Diagnostic diagnostic, string additionalKey1 = null, string additionalKey2 = null)
        {
            return EquivalenceKey.Create(diagnostic, additionalKey1, additionalKey2);
        }

        protected virtual string GetEquivalenceKey(string key, string additionalKey1 = null, string additionalKey2 = null)
        {
            return EquivalenceKey.Create(key, additionalKey1, additionalKey2);
        }

        protected static bool TryFindFirstAncestorOrSelf<TNode>(
            SyntaxNode root,
            TextSpan span,
            out TNode node,
            bool findInsideTrivia = false,
            bool getInnermostNodeForTie = true,
            Func<TNode, bool> predicate = null,
            bool ascendOutOfTrivia = true) where TNode : SyntaxNode
        {
            node = root
                .FindNode(span, findInsideTrivia: findInsideTrivia, getInnermostNodeForTie: getInnermostNodeForTie)?
                .FirstAncestorOrSelf(predicate, ascendOutOfTrivia: ascendOutOfTrivia);

            Assert.NotNull(node);

            return node != null;
        }

        protected static bool TryFindFirstDescendantOrSelf<TNode>(
            SyntaxNode root,
            TextSpan span,
            out TNode node,
            bool findInsideTrivia = false,
            bool getInnermostNodeForTie = true,
            Func<SyntaxNode, bool> descendIntoChildren = null,
            bool descendIntoTrivia = true) where TNode : SyntaxNode
        {
            node = root
                .FindNode(span, findInsideTrivia: findInsideTrivia, getInnermostNodeForTie: getInnermostNodeForTie)?
                .FirstDescendantOrSelf<TNode>(span, descendIntoChildren: descendIntoChildren, descendIntoTrivia: descendIntoTrivia);

            Assert.NotNull(node);

            return node != null;
        }

        protected static bool TryFindNode<TNode>(
            SyntaxNode root,
            TextSpan span,
            out TNode node,
            bool findInsideTrivia = false,
            bool getInnermostNodeForTie = true,
            Func<TNode, bool> predicate = null) where TNode : SyntaxNode
        {
            node = root.FindNode(span, findInsideTrivia: findInsideTrivia, getInnermostNodeForTie: getInnermostNodeForTie) as TNode;

            if (node != null
                && predicate != null
                && !predicate(node))
            {
                node = null;
            }

            Assert.NotNull(node);

            return node != null;
        }

        protected static bool TryFindToken(
            SyntaxNode root,
            int position,
            out SyntaxToken token,
            bool findInsideTrivia = true)
        {
            token = root.FindToken(position, findInsideTrivia: findInsideTrivia);

            bool success = token != default(SyntaxToken);

            Assert.True(success, nameof(token));

            return success;
        }

        protected static bool TryFindTrivia(
            SyntaxNode root,
            int position,
            out SyntaxTrivia trivia,
            bool findInsideTrivia = true)
        {
            trivia = root.FindTrivia(position, findInsideTrivia: findInsideTrivia);

            bool success = trivia != default(SyntaxTrivia);

            Assert.True(success, nameof(trivia));

            return success;
        }

        private static class Assert
        {
            [Conditional("DEBUG")]
            public static void True(bool condition, string name, SyntaxKind kind = SyntaxKind.None)
            {
                Debug.Assert(condition, (kind == SyntaxKind.None) ? $"{name} is {kind}" : $"{name} is not {kind}");
            }

            [Conditional("DEBUG")]
            public static void NotNull<T>(T value) where T : class
            {
                Debug.Assert(value != null, $"{nameof(value)} is null");
            }

            [Conditional("DEBUG")]
            public static void HasNotAnnotation(SyntaxNode node, SyntaxAnnotation annotation)
            {
                Debug.Assert(!node.HasAnnotation(annotation), GetAnnotationMessage(nameof(node), annotation));
            }

            [Conditional("DEBUG")]
            public static void HasNotAnnotation(SyntaxToken token, SyntaxAnnotation annotation)
            {
                Debug.Assert(!token.HasAnnotation(annotation), GetAnnotationMessage(nameof(token), annotation));
            }

            private static string GetAnnotationMessage(string name, SyntaxAnnotation annotation)
            {
                return $"{name} contains annotation - Kind: {annotation.Kind}, Data: {annotation.Data}";
            }
        }
    }
}
