// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring.MissingNodeInList
{
    internal abstract class NodeInListRefactoring<TSyntax, TSyntaxList>
        where TSyntax : SyntaxNode
        where TSyntaxList : SyntaxNode
    {
        public NodeInListRefactoring(TSyntaxList nodeList)
        {
            Nodes = GetNodes(nodeList);
        }

        public SeparatedSyntaxList<TSyntax> Nodes { get; }

        public abstract SeparatedSyntaxList<TSyntax> GetNodes(TSyntaxList nodeList);

        public abstract SyntaxToken GetCloseParenToken(TSyntaxList nodeList);

        public abstract ArgumentOrParameterSyntaxRewriter<TSyntax> GetRewriter(TSyntax node, TSyntax newNode, SyntaxToken tokenBefore, SyntaxToken tokenAfter);

        protected abstract string GetTitle();

        public void ComputeRefactoring(RefactoringContext context, TSyntaxList nodeList)
        {
            int index = FindMissingNode(nodeList, context.Span);

            if (index != -1)
            {
                context.RegisterRefactoring(
                    GetTitle(),
                    cancellationToken => RefactorAsync(context.Document, index, nodeList, cancellationToken));
            }
        }

        private SyntaxToken GetTokenAfter(TSyntaxList nodeList, SeparatedSyntaxList<TSyntax> nodes, int index)
        {
            return (index == nodes.Count - 1)
                ? GetCloseParenToken(nodeList)
                : nodes.GetSeparator(index);
        }

        protected virtual int FindMissingNode(TSyntaxList nodeList, TextSpan span)
        {
            SeparatedSyntaxList<TSyntax> nodes = Nodes;

            int i = 0;
            foreach (TSyntax node in nodes)
            {
                if (i > 0
                    && node.IsMissing)
                {
                    SyntaxToken tokenBefore = nodes.GetSeparator(i - 1);

                    if (span.Start >= tokenBefore.Span.End)
                    {
                        SyntaxToken tokenAfter = GetTokenAfter(nodeList, nodes, i);

                        if (span.End <= tokenAfter.Span.Start)
                            return i;

                        return -1;
                    }
                }

                i++;
            }

            return -1;
        }

        private async Task<Document> RefactorAsync(
            Document document,
            int nodeIndex,
            TSyntaxList nodeList,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SeparatedSyntaxList<TSyntax> nodes = Nodes;

            TSyntax node = nodes[nodeIndex];
            SyntaxToken tokenBefore = nodes.GetSeparator(nodeIndex - 1);
            SyntaxToken tokenAfter = GetTokenAfter(nodeList, nodes, nodeIndex);
            TSyntax newNode = GetNewNode(node, nodes[nodeIndex - 1], tokenBefore, tokenAfter);

            SyntaxNode newArgumentList = Rewrite(nodeList, node, newNode, tokenBefore, tokenAfter);

            SyntaxNode newRoot = root.ReplaceNode(nodeList, newArgumentList);

            return document.WithSyntaxRoot(newRoot);
        }

        protected virtual SyntaxNode Rewrite(TSyntaxList nodeList, TSyntax node, TSyntax newNode, SyntaxToken tokenBefore, SyntaxToken tokenAfter)
        {
            ArgumentOrParameterSyntaxRewriter<TSyntax> rewriter = GetRewriter(node, newNode, tokenBefore, tokenAfter);

            return rewriter.Visit(nodeList);
        }

        protected virtual TSyntax GetNewNode(TSyntax node, TSyntax newNode, SyntaxToken tokenBefore, SyntaxToken tokenAfter)
        {
            SyntaxTriviaList leadingTrivia = tokenBefore
                .TrailingTrivia
                .AddRange(tokenAfter.LeadingTrivia);

            return newNode
                .WithLeadingTrivia(leadingTrivia)
                .WithTrailingTrivia()
                .WithFormatterAnnotation();
        }
    }
}
