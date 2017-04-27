// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidSingleLineBlockRefactoring
    {
        public static void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;

            if (!(block.Parent is AccessorDeclarationSyntax)
                && !(block.Parent is AnonymousFunctionExpressionSyntax))
            {
                SyntaxList<StatementSyntax> statements = block.Statements;

                if (statements.Any())
                {
                    SyntaxToken openBrace = block.OpenBraceToken;

                    if (!openBrace.IsMissing)
                    {
                        SyntaxToken closeBrace = block.CloseBraceToken;

                        if (!closeBrace.IsMissing
                            && block.SyntaxTree.IsSingleLineSpan(TextSpan.FromBounds(openBrace.SpanStart, closeBrace.Span.End), context.CancellationToken))
                        {
                            context.ReportDiagnostic(DiagnosticDescriptors.AvoidSingleLineBlock, block);
                        }
                    }
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            SyntaxToken closeBrace = block.CloseBraceToken;

            BlockSyntax newBlock = block
                .WithCloseBraceToken(closeBrace.WithLeadingTrivia(closeBrace.LeadingTrivia.Add(CSharpFactory.NewLine())))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }
    }
}
