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
        public static void Analyze(SyntaxNodeAnalysisContext context, SyntaxList<StatementSyntax> statements)
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
                    Analyze(context, ((BlockSyntax)statements[i]).Statements);

                previousIndex = statements[i].GetSpanEndLine();
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            StatementSyntax newStatement = statement
                .WithLeadingTrivia(statement.GetLeadingTrivia().Insert(0, CSharpFactory.NewLineTrivia()))
                .WithFormatterAnnotation();

            if (statement.Parent.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement.Parent;

                if (block.IsSingleLine(includeExteriorTrivia: false))
                {
                    SyntaxTriviaList triviaList = block.CloseBraceToken.LeadingTrivia
                        .Add(CSharpFactory.NewLineTrivia());

                    BlockSyntax newBlock = block
                        .WithCloseBraceToken(block.CloseBraceToken.WithLeadingTrivia(triviaList))
                        .WithStatements(block.Statements.Replace(statement, newStatement))
                        .WithFormatterAnnotation();

                    root = root.ReplaceNode(block, newBlock);
                }
                else
                {
                    root = root.ReplaceNode(statement, newStatement);
                }
            }
            else
            {
                root = root.ReplaceNode(statement, newStatement);
            }

            return document.WithSyntaxRoot(root);
        }
    }
}
