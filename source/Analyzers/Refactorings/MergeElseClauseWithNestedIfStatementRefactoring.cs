// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeElseClauseWithNestedIfStatementRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, ElseClauseSyntax elseClause)
        {
            if (!(elseClause.Statement is BlockSyntax block))
                return;

            if (!(block.Statements.SingleOrDefault(shouldThrow: false) is IfStatementSyntax ifStatement))
                return;

            if (!CheckTrivia(elseClause, block, ifStatement))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.MergeElseClauseWithNestedIfStatement, block);
            context.ReportBraces(DiagnosticDescriptors.MergeElseClauseWithNestedIfStatementFadeOut, block);
        }

        private static bool CheckTrivia(ElseClauseSyntax elseClause, BlockSyntax block, IfStatementSyntax ifStatement)
        {
            if (!elseClause.ElseKeyword.TrailingTrivia.IsEmptyOrWhitespace())
                return false;

            if (!block.OpenBraceToken.LeadingTrivia.IsEmptyOrWhitespace())
                return false;

            if (!block.OpenBraceToken.TrailingTrivia.IsEmptyOrWhitespace())
                return false;

            if (!ifStatement.IfKeyword.LeadingTrivia.IsEmptyOrWhitespace())
                return false;

            if (!ifStatement.GetTrailingTrivia().IsEmptyOrWhitespace())
                return false;

            if (!block.CloseBraceToken.LeadingTrivia.IsEmptyOrWhitespace())
                return false;

            return true;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ElseClauseSyntax elseClause,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var block = (BlockSyntax)elseClause.Statement;

            var ifStatement = (IfStatementSyntax)block.Statements[0];

            ElseClauseSyntax newElseClause = elseClause
                .WithStatement(ifStatement)
                .WithElseKeyword(elseClause.ElseKeyword.WithoutTrailingTrivia())
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(elseClause, newElseClause, cancellationToken);
        }
    }
}
