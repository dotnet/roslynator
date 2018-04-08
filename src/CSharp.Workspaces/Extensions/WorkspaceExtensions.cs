// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    /// <summary>
    /// A set of extension methods for the workspace layer.
    /// </summary>
    public static class WorkspaceExtensions
    {
        #region Document
        /// <summary>
        /// Creates a new document updated with the specified text change.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="textChange"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document updated with the specified text changes.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="textChanges"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document updated with the specified text changes.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="textChanges"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document with the specified old node replaced with a new node.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="oldNode"></param>
        /// <param name="newNode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document with the specified old node replaced with new nodes.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="oldNode"></param>
        /// <param name="newNodes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document with the specified old nodes replaced with new nodes.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="document"></param>
        /// <param name="nodes"></param>
        /// <param name="computeReplacementNode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<Document> ReplaceNodesAsync<TNode>(
            this Document document,
            IEnumerable<TNode> nodes,
            Func<TNode, TNode, SyntaxNode> computeReplacementNode,
            CancellationToken cancellationToken = default(CancellationToken)) where TNode : SyntaxNode
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

            if (computeReplacementNode == null)
                throw new ArgumentNullException(nameof(computeReplacementNode));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceNodes(nodes, computeReplacementNode);

            return document.WithSyntaxRoot(newRoot);
        }

        /// <summary>
        /// Creates a new document with the specified old token replaced with a new token.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="oldToken"></param>
        /// <param name="newToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document with the specified old token replaced with new tokens.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="oldToken"></param>
        /// <param name="newTokens"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document with the specified old trivia replaced with a new trivia.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="oldTrivia"></param>
        /// <param name="newTrivia"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document with the specified old trivia replaced with a new trivia.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="oldTrivia"></param>
        /// <param name="newTrivia"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document with a new node inserted before the specified node.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="nodeInList"></param>
        /// <param name="newNode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document with new nodes inserted before the specified node.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="nodeInList"></param>
        /// <param name="newNodes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document with a new node inserted after the specified node.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="nodeInList"></param>
        /// <param name="newNode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document with new nodes inserted after the specified node.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="nodeInList"></param>
        /// <param name="newNodes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document with the specified node removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="node"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new document with the specified nodes removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="nodes"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
        #endregion Document

        #region Solution
        /// <summary>
        /// Creates a new solution with the specified old node replaced with a new node.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="solution"></param>
        /// <param name="oldNode"></param>
        /// <param name="newNode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<Solution> ReplaceNodeAsync<TNode>(
            this Solution solution,
            TNode oldNode,
            TNode newNode,
            CancellationToken cancellationToken = default(CancellationToken)) where TNode : SyntaxNode
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            if (oldNode == null)
                throw new ArgumentNullException(nameof(oldNode));

            if (newNode == null)
                throw new ArgumentNullException(nameof(newNode));

            Document document = solution.GetDocument(oldNode.SyntaxTree);

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceNode(oldNode, newNode);

            return solution.WithDocumentSyntaxRoot(document.Id, newRoot);
        }

        /// <summary>
        /// Creates a new solution with the specified old nodes replaced with new nodes.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="solution"></param>
        /// <param name="nodes"></param>
        /// <param name="computeReplacementNodes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<Solution> ReplaceNodesAsync<TNode>(
            this Solution solution,
            IEnumerable<TNode> nodes,
            Func<TNode, TNode, SyntaxNode> computeReplacementNodes,
            CancellationToken cancellationToken = default(CancellationToken)) where TNode : SyntaxNode
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

            if (computeReplacementNodes == null)
                throw new ArgumentNullException(nameof(computeReplacementNodes));

            Solution newSolution = solution;

            foreach (IGrouping<SyntaxTree, TNode> grouping in nodes.GroupBy(f => f.SyntaxTree))
            {
                Document document = solution.GetDocument(grouping.Key);

                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                SyntaxNode newRoot = root.ReplaceNodes(grouping, computeReplacementNodes);

                newSolution = newSolution.WithDocumentSyntaxRoot(document.Id, newRoot);
            }

            return newSolution;
        }
        #endregion Solution
    }
}
