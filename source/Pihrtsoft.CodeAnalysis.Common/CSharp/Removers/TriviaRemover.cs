// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.Removers
{
    public sealed class TriviaRemover : CSharpSyntaxRewriter
    {
        private static readonly TriviaRemover _instance = new TriviaRemover();

        private TriviaRemover()
        {
        }

        public static SyntaxNode RemoveFrom(SyntaxNode node)
            => _instance.Visit(node);

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
            => CSharpFactory.EmptyTrivia;
    }
}
