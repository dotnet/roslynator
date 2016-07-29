// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Removers
{
    public sealed class RegionRemover : CSharpSyntaxRewriter
    {
        private static readonly RegionRemover _instance = new RegionRemover();

        private RegionRemover()
            : base(visitIntoStructuredTrivia: true)
        {
        }

        public static CompilationUnitSyntax RemoveFrom(CompilationUnitSyntax compilationUnit)
        {
            return (CompilationUnitSyntax)_instance.Visit(compilationUnit);
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.IsKind(
                SyntaxKind.RegionDirectiveTrivia,
                SyntaxKind.EndRegionDirectiveTrivia))
            {
                return CSharpFactory.NewLine;
            }

            return base.VisitTrivia(trivia);
        }
    }
}
