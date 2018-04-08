// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings.NodeInList
{
    internal abstract class NodeInListRefactoring<TSyntax, TListSyntax>
        where TSyntax : SyntaxNode
        where TListSyntax : SyntaxNode
    {
        protected NodeInListRefactoring(TListSyntax listSyntax, SeparatedSyntaxList<TSyntax> list)
        {
            ListSyntax = listSyntax;
            List = list;
        }

        public TListSyntax ListSyntax { get; }

        public SeparatedSyntaxList<TSyntax> List { get; }

        public abstract SyntaxToken GetOpenParenToken();

        public abstract SyntaxToken GetCloseParenToken();

        protected abstract NodeSyntaxRewriter<TSyntax> GetRewriter(RewriterInfo<TSyntax> info);

        protected virtual SyntaxToken GetTokenBefore(int index)
        {
            return (index == 0)
                ? GetOpenParenToken()
                : GetSeparator(index - 1);
        }

        protected virtual SyntaxToken GetTokenAfter(int index)
        {
            return (index == List.Count - 1)
                ? GetCloseParenToken()
                : GetSeparator(index);
        }

        protected virtual int FindNode(TextSpan span)
        {
            SeparatedSyntaxList<TSyntax> nodes = List;

            int i = 0;
            foreach (TSyntax node in nodes)
            {
                if (node.IsMissing)
                {
                    SyntaxToken tokenBefore = GetTokenBefore(i);

                    if (span.Start >= tokenBefore.Span.End)
                    {
                        SyntaxToken tokenAfter = GetTokenAfter(i);

                        if (span.End <= tokenAfter.SpanStart)
                            return i;

                        return -1;
                    }
                }

                i++;
            }

            return -1;
        }

        protected virtual SyntaxNode Rewrite(RewriterInfo<TSyntax> info)
        {
            NodeSyntaxRewriter<TSyntax> rewriter = GetRewriter(info);

            return rewriter.Visit(ListSyntax);
        }

        public SyntaxToken GetSeparator(int index)
        {
            return List.GetSeparator(index);
        }
    }
}
