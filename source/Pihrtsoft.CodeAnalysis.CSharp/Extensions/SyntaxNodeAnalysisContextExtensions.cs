// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    internal static class SyntaxNodeAnalysisContextExtensions
    {
        [DebuggerStepThrough]
        public static void ReportDiagnostic(
            this SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            Location location,
            params object[] messageArgs)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(descriptor, location, messageArgs));
        }

        [DebuggerStepThrough]
        public static INamedTypeSymbol GetTypeByMetadataName(
            this SyntaxNodeAnalysisContext context,
            string fullyQualifiedMetadataName)
        {
            return context.SemanticModel.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
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
    }
}
