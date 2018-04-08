// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Refactorings.NodeInList
{
    internal class RewriterInfo<TSyntax> where TSyntax : SyntaxNode
    {
        public RewriterInfo(TSyntax node, TSyntax newNode, SyntaxToken tokenBefore, SyntaxToken tokenAfter)
        {
            Node = node;
            NewNode = SetTrivia(newNode, tokenBefore, tokenAfter);
            TokenBefore = tokenBefore;
            TokenAfter = tokenAfter;
        }

        public TSyntax Node { get; }

        public TSyntax NewNode { get; }

        public SyntaxToken TokenBefore { get; }

        public SyntaxToken TokenAfter { get; }

        private static TSyntax SetTrivia(TSyntax newNode, SyntaxToken tokenBefore, SyntaxToken tokenAfter)
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
