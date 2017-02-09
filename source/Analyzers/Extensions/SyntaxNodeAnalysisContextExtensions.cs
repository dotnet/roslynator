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

        public static void ReportToken(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken token)
        {
            if (!token.IsMissing)
                context.ReportDiagnostic(descriptor, token);
        }

        public static void ReportNode(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode node)
        {
            if (!node.IsMissing)
                context.ReportDiagnostic(descriptor, node);
        }

        public static void ReportBraces(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, BlockSyntax block)
        {
            ReportToken(context, descriptor, block.OpenBraceToken);
            ReportToken(context, descriptor, block.CloseBraceToken);
        }

        public static void ReportBraces(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, AccessorListSyntax accessorList)
        {
            ReportToken(context, descriptor, accessorList.OpenBraceToken);
            ReportToken(context, descriptor, accessorList.CloseBraceToken);
        }

        public static void ReportParentheses(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ArgumentListSyntax argumentList)
        {
            ReportToken(context, descriptor, argumentList.OpenParenToken);
            ReportToken(context, descriptor, argumentList.CloseParenToken);
        }
    }
}
