// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class SyntaxRemover
    {
        public static TNode RemoveComment<TNode>(TNode node, CommentRemoveOptions removeOptions) where TNode : SyntaxNode
        {
            CommentRemover remover = CommentRemover.Create(node, removeOptions);

            return (TNode)remover.Visit(node);
        }

        public static TNode RemoveComment<TNode>(TNode node, CommentRemoveOptions removeOptions, TextSpan span) where TNode : SyntaxNode
        {
            CommentRemover remover = CommentRemover.Create(node, removeOptions, span);

            return (TNode)remover.Visit(node);
        }

        public static ArgumentListSyntax RemoveNameColon(ArgumentListSyntax argumentList)
        {
            return RemoveNameColon(argumentList, ImmutableArray<ArgumentSyntax>.Empty);
        }

        public static ArgumentListSyntax RemoveNameColon(
            ArgumentListSyntax argumentList,
            ImmutableArray<ArgumentSyntax> arguments)
        {
            if (argumentList == null)
                throw new ArgumentNullException(nameof(argumentList));

            if (arguments == null)
            {
                return (ArgumentListSyntax)NameColonRemover.Instance.Visit(argumentList);
            }
            else
            {
                var remover = new NameColonRemover(arguments);

                return (ArgumentListSyntax)remover.Visit(argumentList);
            }
        }

        public static TNode RemoveRegion<TNode>(TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return (TNode)RegionRemover.Instance.Visit(node);
        }

        public static TNode RemoveTrivia<TNode>(SyntaxNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return (TNode)TriviaRemover.Instance.Visit(node);
        }

        public static TNode RemoveWhitespaceOrEndOfLine<TNode>(TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return (TNode)WhitespaceOrEndOfLineRemover.Instance.Visit(node);
        }

        public static TNode RemoveWhitespaceOrEndOfLine<TNode>(TNode node, TextSpan span) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var remover = new WhitespaceOrEndOfLineRemover(span);

            return (TNode)remover.Visit(node);
        }
    }
}
