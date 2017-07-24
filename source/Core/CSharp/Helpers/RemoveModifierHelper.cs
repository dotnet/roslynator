// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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

        public static TNode RemoveModifierAt<TNode>(TNode node, int index) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTokenList modifiers = node.GetModifiers();

            return RemoveModifier(node, modifiers, modifiers[index], index);
        }

        private static TNode RemoveModifier<TNode>(
            TNode node,
            SyntaxTokenList modifiers,
            SyntaxToken modifier,
            int index) where TNode : SyntaxNode
        {
            SyntaxTriviaList leading = modifier.LeadingTrivia;
            SyntaxTriviaList trailing = modifier.TrailingTrivia;

            if (modifiers.Count == 1)
            {
                SyntaxToken nextToken = modifier.GetNextToken();

                if (!nextToken.IsKind(SyntaxKind.None))
                {
                    SyntaxTriviaList trivia = leading.AddIfNotEmptyOrWhitespace(trailing, nextToken.LeadingTrivia);

                    node = node.ReplaceToken(nextToken, nextToken.WithLeadingTrivia(trivia));
                }
                else
                {
                    SyntaxToken previousToken = modifier.GetPreviousToken();

                    if (!previousToken.IsKind(SyntaxKind.None))
                    {
                        SyntaxTriviaList trivia = previousToken.TrailingTrivia.AddIfNotEmptyOrWhitespace(leading, trailing);

                        node = node.ReplaceToken(previousToken, previousToken.WithTrailingTrivia(trivia));
                    }
                }
            }
            else
            {
                if (index == 0)
                {
                    SyntaxToken nextModifier = modifiers[index + 1];

                    SyntaxTriviaList trivia = leading.AddIfNotEmptyOrWhitespace(trailing, nextModifier.LeadingTrivia);

                    modifiers = modifiers.Replace(nextModifier, nextModifier.WithLeadingTrivia(trivia));
                }
                else
                {
                    SyntaxToken previousModifier = modifiers[index - 1];

                    SyntaxTriviaList trivia = previousModifier.TrailingTrivia.AddIfNotEmptyOrWhitespace(leading, trailing);

                    modifiers = modifiers.Replace(previousModifier, previousModifier.WithTrailingTrivia(trivia));
                }
            }

            modifiers = modifiers.RemoveAt(index);

            return (TNode)node.WithModifiers(modifiers);
        }

        private static SyntaxTriviaList AddIfNotEmptyOrWhitespace(this SyntaxTriviaList trivia, SyntaxTriviaList triviaToAdd)
        {
            return (triviaToAdd.IsEmptyOrWhitespace()) ? trivia : trivia.AddRange(triviaToAdd);
        }

        private static SyntaxTriviaList AddIfNotEmptyOrWhitespace(this SyntaxTriviaList trivia, SyntaxTriviaList triviaToAdd1, SyntaxTriviaList triviaToAdd2)
        {
            return trivia
                .AddIfNotEmptyOrWhitespace(triviaToAdd1)
                .AddIfNotEmptyOrWhitespace(triviaToAdd2);
        }
    }
}
