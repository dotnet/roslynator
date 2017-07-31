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
    internal static class AddEmptyLineAfterLastStatementInDoStatementRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, DoStatementSyntax doStatement)
        {
            StatementSyntax statement = doStatement.Statement;

            if (statement?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)statement;

                SyntaxList<StatementSyntax> statements = block.Statements;

                if (statements.Any())
                {
                    SyntaxToken closeBrace = block.CloseBraceToken;

                    if (!closeBrace.IsMissing)
                    {
                        SyntaxToken whileKeyword = doStatement.WhileKeyword;

                        if (!whileKeyword.IsMissing)
                        {
                            int closeBraceLine = closeBrace.GetSpanEndLine();

                            if (closeBraceLine == whileKeyword.GetSpanStartLine())
                            {
                                StatementSyntax last = statements.Last();

                                int line = last.GetSpanEndLine(context.CancellationToken);

                                if (closeBraceLine - line == 1)
                                {
                                    SyntaxTrivia trivia = last
                                        .GetTrailingTrivia()
                                        .FirstOrDefault(f => f.IsEndOfLineTrivia());

                                    if (trivia.IsEndOfLineTrivia())
                                    {
                                        context.ReportDiagnostic(
                                            DiagnosticDescriptors.AddEmptyLineAfterLastStatementInDoStatement,
                                            Location.Create(doStatement.SyntaxTree, trivia.Span));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTriviaList trailingTrivia = statement.GetTrailingTrivia();

            int index = trailingTrivia.IndexOf(SyntaxKind.EndOfLineTrivia);

            SyntaxTriviaList newTrailingTrivia = trailingTrivia.Insert(index, CSharpFactory.NewLine());

            StatementSyntax newStatement = statement.WithTrailingTrivia(newTrailingTrivia);

            return document.ReplaceNodeAsync(statement, newStatement, cancellationToken);
        }
    }
}
