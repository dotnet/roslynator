// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.Removers
{
    public sealed class RegionRemover : CSharpSyntaxRewriter
    {
        internal static readonly RegionRemover Instance = new RegionRemover();

        public RegionRemover()
            : base(visitIntoStructuredTrivia: true)
        {
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
