// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveBracesRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

            if (!node.IsKind(SyntaxKind.IfStatement)
                || ((IfStatementSyntax)node).IsSimpleIf())
            {
                BlockSyntax block = EmbeddedStatementHelper.AnalyzeBlockToEmbeddedStatement(node);

                if (block != null)
                {
                    SyntaxToken openBrace = block.OpenBraceToken;
                    SyntaxToken closeBrace = block.CloseBraceToken;

                    if (!openBrace.IsMissing
                        && !closeBrace.IsMissing
                        && openBrace.LeadingTrivia.IsEmptyOrWhitespace()
                        && openBrace.TrailingTrivia.IsEmptyOrWhitespace()
                        && closeBrace.LeadingTrivia.IsEmptyOrWhitespace()
                        && closeBrace.TrailingTrivia.IsEmptyOrWhitespace())
                    {
                        string title = node.GetTitle();

                        context.ReportDiagnostic(DiagnosticDescriptors.RemoveBraces, block, title);

                        context.ReportBraces(DiagnosticDescriptors.RemoveBracesFadeOut, block, title);
                    }
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            StatementSyntax statement = block
                .Statements[0]
                .TrimLeadingTrivia()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(block, statement, cancellationToken);
        }
    }
}
