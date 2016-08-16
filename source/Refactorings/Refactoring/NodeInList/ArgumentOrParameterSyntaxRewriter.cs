// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring.MissingNodeInList
{
    internal class ArgumentOrParameterSyntaxRewriter<TSyntax> : CSharpSyntaxRewriter where TSyntax : SyntaxNode
    {
        public ArgumentOrParameterSyntaxRewriter(TSyntax node, TSyntax newNode, SyntaxToken tokenBefore, SyntaxToken tokenAfter)
        {
            Node = node;
            NewNode = newNode;
            TokenBefore = tokenBefore;
            TokenAfter = tokenAfter;
        }

        public TSyntax Node { get; }
        public TSyntax NewNode { get; }
        public SyntaxToken TokenBefore { get; }
        public SyntaxToken TokenAfter { get; }

        public override SyntaxToken VisitToken(SyntaxToken token)
        {
            if (token == TokenBefore)
                return token.WithTrailingTrivia();

            if (token == TokenAfter)
                return token.WithLeadingTrivia();

            return base.VisitToken(token);
        }
    }
}
