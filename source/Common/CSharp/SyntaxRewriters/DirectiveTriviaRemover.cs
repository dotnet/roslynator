// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters
{
    public sealed class DirectiveTriviaRemover : CSharpSyntaxRewriter
    {
        private readonly ImmutableArray<SyntaxTrivia> _trivia;

        public DirectiveTriviaRemover(ImmutableArray<DirectiveTriviaSyntax> trivia)
            : base(visitIntoStructuredTrivia: false)
        {
            _trivia = trivia.Select(f => f.ParentTrivia).ToImmutableArray();
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (_trivia.Contains(trivia))
                return CSharpFactory.NewLine;

            return base.VisitTrivia(trivia);
        }
    }
}
