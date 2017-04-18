// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatEachStatementOnSeparateLineRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, BlockSyntax block)
        {
            Analyze(context, block.Statements);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, SwitchSectionSyntax switchSection)
        {
            Analyze(context, switchSection.Statements);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxList<StatementSyntax> statements)
        {
            if (statements.Count > 1)
            {
                int previousEndLine = statements[0].GetSpanEndLine();

                for (int i = 1; i < statements.Count; i++)
                {
                    StatementSyntax statement = statements[i];

                    if (!statement.IsKind(SyntaxKind.Block, SyntaxKind.EmptyStatement)
                        && statement.GetSpanStartLine() == previousEndLine)
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.FormatEachStatementOnSeparateLine, statement);
                    }

                    previousEndLine = statement.GetSpanEndLine();
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            StatementSyntax newStatement = statement
                .WithLeadingTrivia(statement.GetLeadingTrivia().Insert(0, CSharpFactory.NewLine()))
                .WithFormatterAnnotation();

            if (statement.IsParentKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement.Parent;

                if (block.IsSingleLine(includeExteriorTrivia: false))
                {
                    SyntaxTriviaList triviaList = block.CloseBraceToken.LeadingTrivia
                        .Add(CSharpFactory.NewLine());

                    BlockSyntax newBlock = block
                        .WithCloseBraceToken(block.CloseBraceToken.WithLeadingTrivia(triviaList))
                        .WithStatements(block.Statements.Replace(statement, newStatement))
                        .WithFormatterAnnotation();

                    return await document.ReplaceNodeAsync(block, newBlock, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return await document.ReplaceNodeAsync(statement, newStatement, cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                return await document.ReplaceNodeAsync(statement, newStatement, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
