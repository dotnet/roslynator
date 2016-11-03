// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.SyntaxRewriters
{
    public sealed class TriviaRemover : CSharpSyntaxRewriter
    {
        internal static readonly TriviaRemover Instance = new TriviaRemover();

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            return CSharpFactory.EmptyWhitespaceTrivia();
        }
    }
}
