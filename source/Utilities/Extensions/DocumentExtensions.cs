// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    public static class DocumentExtensions
    {
        public static async Task<Document> WithTextChange(
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

        public static async Task<Document> WithTextChanges(
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

        public static async Task<Document> WithTextChanges(
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

        public static Task<Solution> RemoveFromSolutionAsync(
            this Document document,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            var tcs = new TaskCompletionSource<Solution>();

            Solution newSolution = document.Project.Solution.RemoveDocument(document.Id);

            tcs.SetResult(newSolution);

            return tcs.Task;
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

        public static async Task<Document> InsertNodeAfterAsync(
            this Document document,
            SyntaxNode nodeInList,
            SyntaxNode newNode,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (newNode == null)
                throw new ArgumentNullException(nameof(newNode));

            return await InsertNodesAfterAsync(document, nodeInList, new SyntaxNode[] { newNode }).ConfigureAwait(false);
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
    }
}
