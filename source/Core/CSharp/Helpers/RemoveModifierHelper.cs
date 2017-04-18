// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.Helpers
{
    internal static class RemoveModifierHelper
    {
        public static TNode RemoveModifier<TNode>(TNode node, SyntaxKind modifierKind) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTokenList modifiers = node.GetModifiers();

            int i = modifiers.IndexOf(modifierKind);

            if (i != -1)
            {
                return RemoveModifier(node, modifiers, modifiers[i], i);
            }
            else
            {
                return node;
            }
        }

        public static TNode RemoveModifier<TNode>(TNode node, SyntaxToken modifier) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTokenList modifiers = node.GetModifiers();

            int i = modifiers.IndexOf(modifier);

            if (i != -1)
            {
                return RemoveModifier(node, modifiers, modifier, i);
            }
            else
            {
                return node;
            }
        }

        private static TNode RemoveModifier<TNode>(TNode node, SyntaxTokenList modifiers, SyntaxToken modifier, int i) where TNode : SyntaxNode
        {
            SyntaxTriviaList trivia = GetTrivia(node, modifiers, modifier, i);

            SyntaxTokenList newModifiers = modifiers.Remove(modifier);

            if (i < modifiers.Count - 1)
            {
                newModifiers = newModifiers.ReplaceAt(i, newModifiers[i].PrependToLeadingTrivia(trivia));
            }
            else
            {
                SyntaxToken nextToken = FindNextToken(node, modifier);

                if (!nextToken.IsKind(SyntaxKind.None))
                {
                    TNode newNode = node.ReplaceToken(nextToken, nextToken.PrependToLeadingTrivia(trivia));

                    return (TNode)newNode.WithModifiers(newModifiers);
                }
            }

            return (TNode)node.WithModifiers(newModifiers);
        }

        private static SyntaxToken FindPreviousToken(SyntaxNode node, SyntaxToken token)
        {
            int position = token.FullSpan.Start - 1;

            if (position >= node.FullSpan.Start)
            {
                return FindToken(node, position);
            }
            else
            {
                node = node.Parent;

                if (node != null)
                {
                    return FindToken(node, position);
                }
                else
                {
                    return default(SyntaxToken);
                }
            }
        }

        private static SyntaxToken FindNextToken(SyntaxNode node, SyntaxToken token)
        {
            return FindToken(node, token.FullSpan.End);
        }

        private static SyntaxToken FindToken(SyntaxNode node, int position)
        {
            SyntaxToken token = node.FindToken(position);

            if (token.IsKind(SyntaxKind.None))
            {
                return node.FindTrivia(position).Token;
            }
            else
            {
                return token;
            }
        }

        private static SyntaxTriviaList GetTrivia(SyntaxNode node, SyntaxTokenList modifiers, SyntaxToken modifier, int i)
        {
            SyntaxTriviaList leading = modifier.LeadingTrivia;
            SyntaxTriviaList trailing = modifier.TrailingTrivia;

            if (leading.Any())
            {
                if (trailing.All(f => f.IsWhitespaceTrivia()))
                {
                    return leading;
                }
                else
                {
                    return leading.Concat(trailing).ToSyntaxTriviaList();
                }
            }
            else
            {
                SyntaxToken previousToken = (i == 0)
                    ? FindPreviousToken(node, modifier)
                    : modifiers[i - 1];

                if (!previousToken.IsKind(SyntaxKind.None)
                    && previousToken.TrailingTrivia.Any()
                    && trailing.All(f => f.IsWhitespaceTrivia()))
                {
                    return default(SyntaxTriviaList);
                }
                else
                {
                    return trailing;
                }
            }
        }
    }
}
