// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp
{
    internal static class DiagnosticsExtensions
    {
        internal static void ReportBraces(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, BlockSyntax block, params object[] messageArgs)
        {
            context.ReportToken(descriptor, block.OpenBraceToken, messageArgs);
            context.ReportToken(descriptor, block.CloseBraceToken, messageArgs);
        }

        internal static void ReportBraces(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, AccessorListSyntax accessorList, params object[] messageArgs)
        {
            context.ReportToken(descriptor, accessorList.OpenBraceToken, messageArgs);
            context.ReportToken(descriptor, accessorList.CloseBraceToken, messageArgs);
        }

        internal static void ReportParentheses(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ArgumentListSyntax argumentList, params object[] messageArgs)
        {
            context.ReportToken(descriptor, argumentList.OpenParenToken, messageArgs);
            context.ReportToken(descriptor, argumentList.CloseParenToken, messageArgs);
        }
    }
}
