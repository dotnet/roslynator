// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp
{
    internal static class SyntaxNodeAnalysisContextExtensions
    {
        public static void FadeOutToken(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken token)
        {
            if (!token.IsMissing)
                context.ReportDiagnostic(descriptor, token.GetLocation());
        }

        public static void FadeOutNode(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode node)
        {
            if (!node.IsMissing)
                context.ReportDiagnostic(descriptor, node.GetLocation());
        }

        public static void FadeOutBraces(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, BlockSyntax block)
        {
            FadeOutToken(context, descriptor, block.OpenBraceToken);
            FadeOutToken(context, descriptor, block.CloseBraceToken);
        }

        public static void FadeOutBraces(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, AccessorListSyntax accessorList)
        {
            FadeOutToken(context, descriptor, accessorList.OpenBraceToken);
            FadeOutToken(context, descriptor, accessorList.CloseBraceToken);
        }
    }
}
