// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class DocumentExtensions
    {
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
    }
}
