// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp.Comparers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal abstract class AbstractModifierHelper<TNode> where TNode : SyntaxNode
    {
        public abstract SyntaxTokenList GetModifiers(TNode node);

        public abstract TNode WithModifiers(TNode node, SyntaxTokenList modifiers);

        public abstract SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(TNode node);

        public TNode InsertModifier(TNode node, SyntaxKind modifierKind, IModifierComparer comparer = null)
        {
            return InsertModifier(node, Token(modifierKind), comparer);
        }

        public TNode InsertModifier(TNode node, SyntaxToken modifier, IModifierComparer comparer = null)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTokenList modifiers = GetModifiers(node);

            Debug.Assert(modifiers.Any() || modifiers == default(SyntaxTokenList), node.ToString());

            if (!modifiers.Any())
            {
                SyntaxNodeOrToken nodeOrToken = FindNodeOrTokenAfterModifiers(node);

                if (!nodeOrToken.IsKind(SyntaxKind.None))
                {
                    SyntaxTriviaList trivia = nodeOrToken.GetLeadingTrivia();

                    if (trivia.Any())
                    {
                        SyntaxTriviaList leadingTrivia = modifier.LeadingTrivia;

                        if (!leadingTrivia.IsSingleElasticMarker())
                            trivia = trivia.AddRange(leadingTrivia);

                        if (nodeOrToken.IsNode)
                        {
                            SyntaxNode node2 = nodeOrToken.AsNode();
                            node = node.ReplaceNode(node2, node2.WithoutLeadingTrivia());
                        }
                        else
                        {
                            SyntaxToken token = nodeOrToken.AsToken();
                            node = node.ReplaceToken(token, token.WithoutLeadingTrivia());
                        }

                        return WithModifiers(node, TokenList(modifier.WithLeadingTrivia(trivia)));
                    }
                }
            }

            return WithModifiers(node, modifiers.InsertModifier(modifier, comparer ?? ModifierComparer.Instance));
        }

        public TNode RemoveModifier(TNode node, SyntaxKind modifierKind)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTokenList modifiers = GetModifiers(node);

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

        public TNode RemoveModifier(TNode node, SyntaxToken modifier)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTokenList modifiers = GetModifiers(node);

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

        public TNode RemoveModifierAt(TNode node, int index)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTokenList modifiers = GetModifiers(node);

            return RemoveModifier(node, modifiers, modifiers[index], index);
        }

        private TNode RemoveModifier(
            TNode node,
            SyntaxTokenList modifiers,
            SyntaxToken modifier,
            int index)
        {
            SyntaxTriviaList leading = modifier.LeadingTrivia;
            SyntaxTriviaList trailing = modifier.TrailingTrivia;

            if (modifiers.Count == 1)
            {
                SyntaxToken nextToken = modifier.GetNextToken();

                if (!nextToken.IsKind(SyntaxKind.None))
                {
                    SyntaxTriviaList trivia = AddIfNotEmptyOrWhitespace(leading, trailing, nextToken.LeadingTrivia);

                    node = node.ReplaceToken(nextToken, nextToken.WithLeadingTrivia(trivia));
                }
                else
                {
                    SyntaxToken previousToken = modifier.GetPreviousToken();

                    if (!previousToken.IsKind(SyntaxKind.None))
                    {
                        SyntaxTriviaList trivia = AddIfNotEmptyOrWhitespace(previousToken.TrailingTrivia, leading, trailing);

                        node = node.ReplaceToken(previousToken, previousToken.WithTrailingTrivia(trivia));
                    }
                }
            }
            else
            {
                if (index == 0)
                {
                    SyntaxToken nextModifier = modifiers[index + 1];

                    SyntaxTriviaList trivia = AddIfNotEmptyOrWhitespace(leading, trailing, nextModifier.LeadingTrivia);

                    modifiers = modifiers.Replace(nextModifier, nextModifier.WithLeadingTrivia(trivia));
                }
                else
                {
                    SyntaxToken previousModifier = modifiers[index - 1];

                    SyntaxTriviaList trivia = AddIfNotEmptyOrWhitespace(previousModifier.TrailingTrivia, leading, trailing);

                    modifiers = modifiers.Replace(previousModifier, previousModifier.WithTrailingTrivia(trivia));
                }
            }

            modifiers = modifiers.RemoveAt(index);

            return WithModifiers(node, modifiers);
        }

        private static SyntaxTriviaList AddIfNotEmptyOrWhitespace(SyntaxTriviaList trivia, SyntaxTriviaList triviaToAdd)
        {
            return (triviaToAdd.IsEmptyOrWhitespace()) ? trivia : trivia.AddRange(triviaToAdd);
        }

        private static SyntaxTriviaList AddIfNotEmptyOrWhitespace(SyntaxTriviaList trivia, SyntaxTriviaList triviaToAdd1, SyntaxTriviaList triviaToAdd2)
        {
            trivia = AddIfNotEmptyOrWhitespace(trivia, triviaToAdd1);

            return AddIfNotEmptyOrWhitespace(trivia, triviaToAdd2);
        }

        public TNode RemoveAccessModifiers(TNode node)
        {
            SyntaxTokenList modifiers = GetModifiers(node);

            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                SyntaxToken modifier = modifiers[i];

                if (modifier.IsAccessModifier())
                    node = RemoveModifier(node, modifiers, modifier, i);
            }

            return node;
        }

        public TNode RemoveModifiers(TNode node)
        {
            SyntaxTokenList modifiers = GetModifiers(node);

            if (!modifiers.Any())
                return node;

            SyntaxToken firstModifier = modifiers.First();

            if (modifiers.Count == 1)
                return RemoveModifier(node, firstModifier);

            SyntaxToken nextToken = modifiers.Last().GetNextToken();

            if (!nextToken.IsKind(SyntaxKind.None))
            {
                SyntaxTriviaList trivia = firstModifier.LeadingTrivia;

                trivia = trivia.AddRange(firstModifier.TrailingTrivia.EmptyIfWhitespace());

                for (int i = 1; i < modifiers.Count; i++)
                    trivia = trivia.AddRange(modifiers[i].GetLeadingAndTrailingTrivia().EmptyIfWhitespace());

                trivia = trivia.AddRange(nextToken.LeadingTrivia.EmptyIfWhitespace());

                node = node.ReplaceToken(nextToken, nextToken.WithLeadingTrivia(trivia));
            }
            else
            {
                SyntaxToken previousToken = firstModifier.GetPreviousToken();

                if (!previousToken.IsKind(SyntaxKind.None))
                {
                    SyntaxTriviaList trivia = firstModifier.GetLeadingAndTrailingTrivia();

                    for (int i = 1; i < modifiers.Count; i++)
                        trivia = trivia.AddRange(modifiers[i].GetLeadingAndTrailingTrivia().EmptyIfWhitespace());

                    node = node.ReplaceToken(nextToken, nextToken.AppendToTrailingTrivia(trivia));
                }
            }

            return WithModifiers(node, default(SyntaxTokenList));
        }
    }
}
