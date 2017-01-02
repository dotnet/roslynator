// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeElseClauseWithNestedIfStatementRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, ElseClauseSyntax elseClause)
        {
            StatementSyntax statement = elseClause.Statement;

            if (statement?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)statement;

                SyntaxList<StatementSyntax> statements = block.Statements;

                if (statements.Count == 1
                    && statements[0].IsKind(SyntaxKind.IfStatement))
                {
                    var ifStatement = (IfStatementSyntax)statements[0];

                    if (ifStatement.Else == null
                        && CheckTrivia(elseClause, ifStatement))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.MergeElseClauseWithNestedIfStatement,
                            block.GetLocation());

                        context.FadeOutBraces(
                            DiagnosticDescriptors.MergeElseClauseWithNestedIfStatementFadeOut,
                            block);
                    }
                }
            }
        }

        private static bool CheckTrivia(ElseClauseSyntax elseClause, IfStatementSyntax ifStatement)
        {
            TextSpan elseSpan = elseClause.Span;
            TextSpan ifSpan = ifStatement.Span;

            TextSpan span = TextSpan.FromBounds(elseSpan.Start, ifSpan.Start);
            TextSpan span2 = TextSpan.FromBounds(ifSpan.End, elseSpan.End);

            foreach (SyntaxTrivia trivia in elseClause.DescendantTrivia())
            {
                TextSpan triviaSpan = trivia.Span;

                if (span.Contains(triviaSpan))
                {
                    if (!trivia.IsWhitespaceOrEndOfLineTrivia())
                        return false;
                }
                else if (span2.Contains(triviaSpan))
                {
                    if (!trivia.IsWhitespaceOrEndOfLineTrivia())
                        return false;
                }
            }

            return true;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ElseClauseSyntax elseClause,
            CancellationToken cancellationToken)
        {
            var block = (BlockSyntax)elseClause.Statement;

            var ifStatement = (IfStatementSyntax)block.Statements[0];

            ElseClauseSyntax newElseClause = elseClause
                .WithStatement(ifStatement)
                .WithElseKeyword(elseClause.ElseKeyword.WithoutTrailingTrivia())
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(elseClause, newElseClause, cancellationToken).ConfigureAwait(false);
        }
    }
}
