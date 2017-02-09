// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using Roslynator.Extensions;
using Roslynator.CSharp.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantBracesRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, BlockSyntax block)
        {
            if (block.IsParentKind(SyntaxKind.Block)
                && block.Statements.Any())
            {
                context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantBraces, block);

                context.ReportBraces(DiagnosticDescriptors.RemoveRedundantBracesFadeOut, block);
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            StatementSyntax[] statements = block.Statements.ToArray();

            SyntaxToken openBrace = block.OpenBraceToken;
            SyntaxTriviaList leadingTrivia = openBrace.LeadingTrivia;

            AddIfNotWhiteSpaceOrEndOfLine(openBrace.TrailingTrivia, ref leadingTrivia);
            AddIfNotWhiteSpaceOrEndOfLine(statements[0].GetLeadingTrivia(), ref leadingTrivia);

            statements[0] = statements[0].WithLeadingTrivia(leadingTrivia);

            SyntaxToken closeBrace = block.CloseBraceToken;
            SyntaxTriviaList trailingTrivia = closeBrace.TrailingTrivia;

            InsertIfNotWhiteSpaceOrEndOfLine(statements[statements.Length - 1].GetTrailingTrivia(), ref trailingTrivia);
            InsertIfNotWhiteSpaceOrEndOfLine(closeBrace.LeadingTrivia, ref trailingTrivia);

            statements[statements.Length - 1] = statements[statements.Length - 1].WithTrailingTrivia(trailingTrivia);

            IEnumerable<StatementSyntax> newStatements = statements.Select(f => f.WithFormatterAnnotation());

            return await document.ReplaceNodeAsync(block, newStatements, cancellationToken).ConfigureAwait(false);
        }

        private static void AddIfNotWhiteSpaceOrEndOfLine(IEnumerable<SyntaxTrivia> trivia, ref SyntaxTriviaList triviaList)
        {
            if (trivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                triviaList = triviaList.AddRange(trivia);
        }

        private static void InsertIfNotWhiteSpaceOrEndOfLine(IEnumerable<SyntaxTrivia> trivia, ref SyntaxTriviaList triviaList)
        {
            if (trivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                triviaList = triviaList.InsertRange(0, trivia);
        }
    }
}
