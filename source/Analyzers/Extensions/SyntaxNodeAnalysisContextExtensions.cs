// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Extensions;

namespace Roslynator.CSharp
{
    internal static class SyntaxNodeAnalysisContextExtensions
    {
        public static SyntaxTree SyntaxTree(this SyntaxNodeAnalysisContext context)
        {
            return context.Node.SyntaxTree;
        }

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

        public static void FadeOutParentheses(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ArgumentListSyntax argumentList)
        {
            FadeOutToken(context, descriptor, argumentList.OpenParenToken);
            FadeOutToken(context, descriptor, argumentList.CloseParenToken);
        }
    }
}
