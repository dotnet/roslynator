// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    internal static class DiagnosticHelper
    {
        public static void AnalyzeStatements(SyntaxNodeAnalysisContext context, SyntaxList<StatementSyntax> statements)
        {
            if (statements.Count == 0)
                return;

            if (statements.Count == 1 && !statements[0].IsKind(SyntaxKind.Block))
                return;

            int previousIndex = statements[0].GetSpanEndLine();

            for (int i = 1; i < statements.Count; i++)
            {
                if (!statements[i].IsKind(SyntaxKind.Block)
                    && !statements[i].IsKind(SyntaxKind.EmptyStatement)
                    && statements[i].GetSpanStartLine() == previousIndex)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.FormatEachStatementOnSeparateLine,
                        statements[i].GetLocation());
                }

                if (statements[i].IsKind(SyntaxKind.Block))
                    AnalyzeStatements(context, ((BlockSyntax)statements[i]).Statements);

                previousIndex = statements[i].GetSpanEndLine();
            }
        }

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
