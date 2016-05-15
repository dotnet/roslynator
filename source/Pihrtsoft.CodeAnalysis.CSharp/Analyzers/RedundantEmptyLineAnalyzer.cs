// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Analyzers
{
    internal static class RedundantEmptyLineAnalyzer
    {
        public static void AnalyzeDeclaration(
            SyntaxNodeAnalysisContext context,
            SyntaxList<MemberDeclarationSyntax> members,
            SyntaxToken openBrace,
            SyntaxToken closeBrace)
        {
            if (members.Count > 0)
            {
                AnalyzeStart(context, members[0], openBrace);
                AnalyzeEnd(context, members[members.Count - 1], closeBrace);
            }
        }

        public static void AnalyzeBlock(SyntaxNodeAnalysisContext context, BlockSyntax block)
        {
            if (block.Statements.Count > 0)
            {
                AnalyzeStart(context, block.Statements[0], block.OpenBraceToken);
                AnalyzeEnd(context, block.Statements[block.Statements.Count - 1], block.CloseBraceToken);
            }
        }

        private static void AnalyzeStart(
            SyntaxNodeAnalysisContext context,
            SyntaxNode node,
            SyntaxToken brace)
        {
            if (!brace.IsMissing
                && (node.GetSpanStartLine() - brace.GetSpanEndLine()) > 1)
            {
                AnalyzeTriviaList(context, node.GetLeadingTrivia(), isEnd: false);
            }
        }

        private static void AnalyzeEnd(
            SyntaxNodeAnalysisContext context,
            SyntaxNode node,
            SyntaxToken brace)
        {
            if (!brace.IsMissing
                && (brace.GetSpanStartLine() - node.GetSpanEndLine()) > 1)
            {
                AnalyzeTriviaList(context, brace.LeadingTrivia, isEnd: true);
            }
        }

        private static void AnalyzeTriviaList(
            SyntaxNodeAnalysisContext context,
            SyntaxTriviaList triviaList,
            bool isEnd)
        {
            int i = 0;

            foreach (SyntaxTrivia trivia in triviaList)
            {
                if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    if (isEnd)
                    {
                        for (int j = i + 1; j < triviaList.Count; j++)
                        {
                            if (!triviaList[j].IsWhitespaceOrEndOfLine())
                                return;
                        }
                    }

                    TextSpan span = TextSpan.FromBounds(triviaList.Span.Start, trivia.Span.End);

                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveRedundantEmptyLine,
                        Location.Create(context.Node.SyntaxTree, span));

                    return;
                }
                else if (!trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    return;
                }

                i++;
            }
        }
    }
}