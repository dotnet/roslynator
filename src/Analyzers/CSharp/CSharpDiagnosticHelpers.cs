// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.DiagnosticHelpers;

namespace Roslynator.CSharp
{
    internal static class CSharpDiagnosticHelpers
    {
        public static void ReportBraces(SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, BlockSyntax block, params object[] messageArgs)
        {
            ReportToken(context, descriptor, block.OpenBraceToken, messageArgs);
            ReportToken(context, descriptor, block.CloseBraceToken, messageArgs);
        }

        public static void ReportBraces(SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, AccessorListSyntax accessorList, params object[] messageArgs)
        {
            ReportToken(context, descriptor, accessorList.OpenBraceToken, messageArgs);
            ReportToken(context, descriptor, accessorList.CloseBraceToken, messageArgs);
        }

        public static void ReportParentheses(SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ArgumentListSyntax argumentList, params object[] messageArgs)
        {
            ReportToken(context, descriptor, argumentList.OpenParenToken, messageArgs);
            ReportToken(context, descriptor, argumentList.CloseParenToken, messageArgs);
        }
    }
}
