// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters
{
    public sealed class RemoveTriviaSyntaxRewriter : CSharpSyntaxRewriter
    {
        private static readonly RemoveTriviaSyntaxRewriter _instance = new RemoveTriviaSyntaxRewriter();

        private RemoveTriviaSyntaxRewriter()
        {
        }

        public static SyntaxNode VisitNode(SyntaxNode node)
            => _instance.Visit(node);

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
            => SyntaxHelper.EmptyTrivia;
    }
}
