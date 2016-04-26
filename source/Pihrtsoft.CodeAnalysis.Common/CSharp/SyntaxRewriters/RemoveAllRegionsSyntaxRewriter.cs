// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters
{
    public class RemoveAllRegionsSyntaxRewriter : CSharpSyntaxRewriter
    {
        private static readonly RemoveAllRegionsSyntaxRewriter _instance = new RemoveAllRegionsSyntaxRewriter();

        private RemoveAllRegionsSyntaxRewriter()
            : base(visitIntoStructuredTrivia: true)
        {
        }

        public static CompilationUnitSyntax VisitNode(CompilationUnitSyntax compilationUnit)
            => (CompilationUnitSyntax)_instance.Visit(compilationUnit);

        public override SyntaxNode VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
            => null;

        public override SyntaxNode VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
            => null;
    }
}
