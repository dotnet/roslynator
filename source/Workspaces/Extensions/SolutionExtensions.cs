// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class SolutionExtensions
    {
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
    }
}
