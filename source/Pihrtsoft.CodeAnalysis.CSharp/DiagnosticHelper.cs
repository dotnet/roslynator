// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    internal static class DiagnosticHelper
    {
        public static void FadeOutToken(SyntaxNodeAnalysisContext context, SyntaxToken token, DiagnosticDescriptor descriptor)
        {
            if (!token.IsMissing)
                context.ReportDiagnostic(descriptor, token.GetLocation());
        }

        public static void FadeOutNode(SyntaxNodeAnalysisContext context, SyntaxNode node, DiagnosticDescriptor descriptor)
        {
            if (!node.IsMissing)
                context.ReportDiagnostic(descriptor, node.GetLocation());
        }

        public static void FadeOutBraces(SyntaxNodeAnalysisContext context, BlockSyntax block, DiagnosticDescriptor descriptor)
        {
            FadeOutToken(context, block.OpenBraceToken, descriptor);
            FadeOutToken(context, block.CloseBraceToken, descriptor);
        }

        public static void FadeOutBraces(SyntaxNodeAnalysisContext context, AccessorListSyntax accessorList, DiagnosticDescriptor descriptor)
        {
            FadeOutToken(context, accessorList.OpenBraceToken, descriptor);
            FadeOutToken(context, accessorList.CloseBraceToken, descriptor);
        }
    }
}
