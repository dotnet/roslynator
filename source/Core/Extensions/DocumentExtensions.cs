// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    public static class DocumentExtensions
    {
        public static async Task<Document> WithTextChangeAsync(
            this Document document,
            TextChange textChange,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = sourceText.WithChanges(new TextChange[] { textChange });

            return document.WithText(newSourceText);
        }

        public static async Task<Document> WithTextChangesAsync(
            this Document document,
            TextChange[] textChanges,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (textChanges == null)
                throw new ArgumentNullException(nameof(textChanges));

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = sourceText.WithChanges(textChanges);

            return document.WithText(newSourceText);
        }

        public static async Task<Document> WithTextChangesAsync(
            this Document document,
            IEnumerable<TextChange> textChanges,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (textChanges == null)
                throw new ArgumentNullException(nameof(textChanges));

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = sourceText.WithChanges(textChanges);

            return document.WithText(newSourceText);
        }

        public static async Task<Document> ReplaceNodeAsync(
            this Document document,
            SyntaxNode oldNode,
            SyntaxNode newNode,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (oldNode == null)
                throw new ArgumentNullException(nameof(oldNode));

            if (newNode == null)
                throw new ArgumentNullException(nameof(newNode));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceNode(oldNode, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ReplaceNodeAsync(
            this Document document,
            SyntaxNode oldNode,
            IEnumerable<SyntaxNode> newNodes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (oldNode == null)
                throw new ArgumentNullException(nameof(oldNode));

            if (newNodes == null)
                throw new ArgumentNullException(nameof(newNodes));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceNode(oldNode, newNodes);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ReplaceTokenAsync(
            this Document document,
            SyntaxToken oldToken,
            SyntaxToken newToken,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceToken(oldToken, newToken);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ReplaceTokenAsync(
            this Document document,
            SyntaxToken oldToken,
            IEnumerable<SyntaxToken> newTokens,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (newTokens == null)
                throw new ArgumentNullException(nameof(newTokens));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceToken(oldToken, newTokens);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ReplaceTriviaAsync(
            this Document document,
            SyntaxTrivia oldTrivia,
            SyntaxTrivia newTrivia,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceTrivia(oldTrivia, newTrivia);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ReplaceTriviaAsync(
            this Document document,
            SyntaxTrivia oldTrivia,
            IEnumerable<SyntaxTrivia> newTrivia,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (newTrivia == null)
                throw new ArgumentNullException(nameof(newTrivia));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceTrivia(oldTrivia, newTrivia);

            return document.WithSyntaxRoot(newRoot);
        }

        public static Task<Document> InsertNodeBeforeAsync(
            this Document document,
            SyntaxNode nodeInList,
            SyntaxNode newNode,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (newNode == null)
                throw new ArgumentNullException(nameof(newNode));

            return InsertNodesBeforeAsync(document, nodeInList, new SyntaxNode[] { newNode }, cancellationToken);
        }

        public static async Task<Document> InsertNodesBeforeAsync(
            this Document document,
            SyntaxNode nodeInList,
            IEnumerable<SyntaxNode> newNodes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (nodeInList == null)
                throw new ArgumentNullException(nameof(nodeInList));

            if (newNodes == null)
                throw new ArgumentNullException(nameof(newNodes));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.InsertNodesBefore(nodeInList, newNodes);

            return document.WithSyntaxRoot(newRoot);
        }

        public static Task<Document> InsertNodeAfterAsync(
            this Document document,
            SyntaxNode nodeInList,
            SyntaxNode newNode,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (newNode == null)
                throw new ArgumentNullException(nameof(newNode));

            return InsertNodesAfterAsync(document, nodeInList, new SyntaxNode[] { newNode }, cancellationToken);
        }

        public static async Task<Document> InsertNodesAfterAsync(
            this Document document,
            SyntaxNode nodeInList,
            IEnumerable<SyntaxNode> newNodes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (nodeInList == null)
                throw new ArgumentNullException(nameof(nodeInList));

            if (newNodes == null)
                throw new ArgumentNullException(nameof(newNodes));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.InsertNodesAfter(nodeInList, newNodes);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> RemoveNodeAsync(
            this Document document,
            SyntaxNode node,
            SyntaxRemoveOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.RemoveNode(node, options);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> RemoveNodesAsync(
            this Document document,
            IEnumerable<SyntaxNode> nodes,
            SyntaxRemoveOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.RemoveNodes(nodes, options);

            return document.WithSyntaxRoot(newRoot);
        }

        internal static Solution Solution(this Document document)
        {
            return document.Project.Solution;
        }

        internal static async Task<ImmutableArray<SyntaxNode>> FindNodesAsync(
            this Document document,
            ISymbol symbol,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (document == null)
                throw new ArgumentNullException(nameof(document));

            List<SyntaxNode> nodes = null;

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            foreach (ReferencedSymbol referencedSymbol in await SymbolFinder.FindReferencesAsync(
                symbol,
                document.Solution(),
                ImmutableHashSet.Create(document),
                cancellationToken).ConfigureAwait(false))
            {
                foreach (ReferenceLocation referenceLocation in referencedSymbol.Locations)
                {
                    if (!referenceLocation.IsImplicit
                        && (allowCandidate || !referenceLocation.IsCandidateLocation))
                    {
                        Location location = referenceLocation.Location;

                        if (location.IsInSource)
                        {
                            SyntaxNode node = root.FindNode(location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);

                            Debug.Assert(node != null);

                            if (node != null)
                                (nodes ?? (nodes = new List<SyntaxNode>())).Add(node);
                        }
                    }
                }
            }

            if (nodes != null)
            {
                return ImmutableArray.CreateRange(nodes);
            }
            else
            {
                return ImmutableArray<SyntaxNode>.Empty;
            }
        }
    }
}
