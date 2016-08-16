// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring.NodeInList
{
    internal class NodeSyntaxRewriter<TSyntax> : CSharpSyntaxRewriter where TSyntax : SyntaxNode
    {
        public NodeSyntaxRewriter(RewriterInfo<TSyntax> info)
        {
            Info = info;
        }

        public RewriterInfo<TSyntax> Info { get; }

        public override SyntaxToken VisitToken(SyntaxToken token)
        {
            if (token == Info.TokenBefore)
                return token.WithTrailingTrivia();

            if (token == Info.TokenAfter)
                return token.WithLeadingTrivia();

            return base.VisitToken(token);
        }
    }
}
