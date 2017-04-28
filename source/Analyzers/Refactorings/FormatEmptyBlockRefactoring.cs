// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatEmptyBlockRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, BlockSyntax block)
        {
            SyntaxList<StatementSyntax> statements = block.Statements;

            if (!statements.Any()
                && !(block.Parent is AccessorDeclarationSyntax)
                && !(block.Parent is AnonymousFunctionExpressionSyntax))
            {
                int startLine = block.OpenBraceToken.GetSpanStartLine();
                int endLine = block.CloseBraceToken.GetSpanEndLine();

                if ((endLine - startLine) != 1
                    && block
                        .DescendantTrivia(block.Span)
                        .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.FormatEmptyBlock, block);
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            BlockSyntax newBlock = block
                .WithOpenBraceToken(block.OpenBraceToken.WithoutTrailingTrivia())
                .WithCloseBraceToken(block.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLine()))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }
    }
}
