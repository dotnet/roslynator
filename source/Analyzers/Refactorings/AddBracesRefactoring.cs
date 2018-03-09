// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddBracesRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, StatementSyntax statement)
        {
            if (!statement.IsKind(SyntaxKind.IfStatement)
                || ((IfStatementSyntax)statement).IsSimpleIf())
            {
                StatementSyntax embeddedStatement = EmbeddedStatementHelper.AnalyzeEmbeddedStatementToBlock(statement, ifInsideElse: false, usingInsideUsing: false);

                if (embeddedStatement != null)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AddBracesWhenExpressionSpansOverMultipleLines,
                        embeddedStatement,
                        statement.GetTitle());
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            BlockSyntax block = SyntaxFactory.Block(statement).WithFormatterAnnotation();

            return document.ReplaceNodeAsync(statement, block, cancellationToken);
        }
    }
}
