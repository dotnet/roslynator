// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddBracesRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, StatementSyntax statement)
        {
            if (!statement.IsKind(SyntaxKind.IfStatement)
                || !IfElseChain.IsPartOfChain((IfStatementSyntax)statement))
            {
                StatementSyntax embeddedStatement = GetEmbeddedStatementThatShouldBeInsideBlock(statement);

                if (embeddedStatement != null)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AddBraces,
                        embeddedStatement.GetLocation(),
                        SyntaxHelper.GetNodeTitle(statement));
                }
            }
        }

        private static StatementSyntax GetEmbeddedStatementThatShouldBeInsideBlock(SyntaxNode node)
        {
            StatementSyntax statement = EmbeddedStatement.GetBlockOrEmbeddedStatement(node);

            if (statement?.IsKind(SyntaxKind.Block) == false)
            {
                if (!statement.IsSingleLine() || !EmbeddedStatement.FormatSupportsEmbeddedStatement(node))
                    return statement;
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            BlockSyntax block = SyntaxFactory.Block(statement).WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(statement, block, cancellationToken).ConfigureAwait(false);
        }
    }
}
