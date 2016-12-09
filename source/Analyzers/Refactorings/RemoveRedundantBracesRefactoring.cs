// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantBracesRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, BlockSyntax block)
        {
            SyntaxList<StatementSyntax> statements = block.Statements;

            if (statements.Count == 1)
            {
                StatementSyntax statement = statements.First();

                if (statement.IsKind(SyntaxKind.Block))
                {
                    var innerBlock = (BlockSyntax)statement;

                    if (block.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && block.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && innerBlock.OpenBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && innerBlock.CloseBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantBraces, innerBlock.GetLocation());

                        context.FadeOutBraces(DiagnosticDescriptors.RemoveRedundantBracesFadeOut, innerBlock);
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            var parent = (BlockSyntax)block.Parent;

            BlockSyntax newNode = parent.ReplaceNode(block, block.Statements)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(parent, newNode).ConfigureAwait(false);
        }
    }
}
