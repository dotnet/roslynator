// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantEmptyLineRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
        {
            AnalyzeDeclaration(
                context,
                classDeclaration.Members,
                classDeclaration.OpenBraceToken,
                classDeclaration.CloseBraceToken);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, StructDeclarationSyntax structDeclaration)
        {
            AnalyzeDeclaration(
                context,
                structDeclaration.Members,
                structDeclaration.OpenBraceToken,
                structDeclaration.CloseBraceToken);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, InterfaceDeclarationSyntax interfaceDeclaration)
        {
            AnalyzeDeclaration(
                context,
                interfaceDeclaration.Members,
                interfaceDeclaration.OpenBraceToken,
                interfaceDeclaration.CloseBraceToken);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, NamespaceDeclarationSyntax namespaceDeclaration)
        {
            SyntaxList<MemberDeclarationSyntax> members = namespaceDeclaration.Members;
            SyntaxList<ExternAliasDirectiveSyntax> externs = namespaceDeclaration.Externs;

            if (externs.Any())
            {
                AnalyzeStart(context, externs.First(), namespaceDeclaration.OpenBraceToken);
            }
            else
            {
                SyntaxList<UsingDirectiveSyntax> usings = namespaceDeclaration.Usings;

                if (usings.Any())
                {
                    AnalyzeStart(context, usings.First(), namespaceDeclaration.OpenBraceToken);
                }
                else if (members.Any())
                {
                    AnalyzeStart(context, members.First(), namespaceDeclaration.OpenBraceToken);
                }
            }

            if (members.Any())
                AnalyzeEnd(context, members.Last(), namespaceDeclaration.CloseBraceToken);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, SwitchStatementSyntax switchStatement)
        {
            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            if (sections.Any())
            {
                AnalyzeStart(context, sections.First(), switchStatement.OpenBraceToken);
                AnalyzeEnd(context, sections.Last(), switchStatement.CloseBraceToken);

                if (sections.Count > 1)
                {
                    SwitchSectionSyntax prevSection = sections.First();

                    for (int i = 1; i < sections.Count; i++)
                    {
                        if (prevSection.Statements.LastOrDefault()?.IsKind(SyntaxKind.Block) == true)
                        {
                            SwitchSectionSyntax section = sections[i];

                            SyntaxTriviaList trailingTrivia = prevSection.GetTrailingTrivia();
                            SyntaxTriviaList leadingTrivia = section.GetLeadingTrivia();

                            if (!IsStandardTriviaBetweenSections(trailingTrivia, leadingTrivia)
                                && switchStatement
                                    .SyntaxTree
                                    .GetLineSpan(TextSpan.FromBounds(prevSection.Span.End, section.Span.Start), context.CancellationToken)
                                    .GetLineCount() == 3)
                            {
                                SyntaxTrivia trivia = leadingTrivia
                                    .SkipWhile(f => f.IsWhitespaceTrivia())
                                    .FirstOrDefault();

                                if (trivia.IsEndOfLineTrivia()
                                    && trailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                                    && leadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.RemoveRedundantEmptyLine,
                                        Location.Create(switchStatement.SyntaxTree, TextSpan.FromBounds(section.FullSpan.Start, trivia.Span.End)));
                                }
                            }
                        }

                        prevSection = sections[i];
                    }
                }
            }
        }

        private static void AnalyzeDeclaration(
            SyntaxNodeAnalysisContext context,
            SyntaxList<MemberDeclarationSyntax> members,
            SyntaxToken openBrace,
            SyntaxToken closeBrace)
        {
            if (members.Any())
            {
                AnalyzeStart(context, members.First(), openBrace);
                AnalyzeEnd(context, members.Last(), closeBrace);
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, BlockSyntax block)
        {
            SyntaxList<StatementSyntax> statements = block.Statements;

            if (statements.Any())
            {
                AnalyzeStart(context, statements.First(), block.OpenBraceToken);
                AnalyzeEnd(context, statements.Last(), block.CloseBraceToken);
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
                TextSpan? span = GetEmptyLineSpan(node.GetLeadingTrivia(), isEnd: false);

                if (span != null)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveRedundantEmptyLine,
                        Location.Create(context.Node.SyntaxTree, span.Value));
                }
            }
        }

        private static void AnalyzeEnd(
            SyntaxNodeAnalysisContext context,
            SyntaxNode node,
            SyntaxToken brace)
        {
            if (!brace.IsMissing)
            {
                int braceLine = brace.GetSpanStartLine();

                if (braceLine - node.GetSpanEndLine() > 1)
                {
                    TextSpan? span = GetEmptyLineSpan(brace.LeadingTrivia, isEnd: true);

                    if (span != null
                        && !IsEmptyLastLineInDoStatement(node, braceLine, span.Value))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.RemoveRedundantEmptyLine,
                            Location.Create(context.Node.SyntaxTree, span.Value));
                    }
                }
            }
        }

        private static bool IsEmptyLastLineInDoStatement(
            SyntaxNode node,
            int closeBraceLine,
            TextSpan span)
        {
            SyntaxNode parent = node.Parent;

            if (parent?.IsKind(SyntaxKind.Block) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.DoStatement) == true)
                {
                    var doStatement = (DoStatementSyntax)parent;

                    int emptyLine = doStatement.SyntaxTree.GetLineSpan(span).EndLine();

                    if (emptyLine == closeBraceLine)
                    {
                        int whileKeywordLine = doStatement.WhileKeyword.GetSpanStartLine();

                        if (closeBraceLine == whileKeywordLine)
                            return true;
                    }
                }
            }

            return false;
        }

        private static TextSpan? GetEmptyLineSpan(
            SyntaxTriviaList triviaList,
            bool isEnd)
        {
            int i = 0;

            foreach (SyntaxTrivia trivia in triviaList)
            {
                if (trivia.IsEndOfLineTrivia())
                {
                    if (isEnd)
                    {
                        for (int j = i + 1; j < triviaList.Count; j++)
                        {
                            if (!triviaList[j].IsWhitespaceOrEndOfLineTrivia())
                                return null;
                        }
                    }

                    return TextSpan.FromBounds(triviaList.Span.Start, trivia.Span.End);
                }
                else if (!trivia.IsWhitespaceTrivia())
                {
                    return null;
                }

                i++;
            }

            return null;
        }

        private static bool IsStandardTriviaBetweenSections(SyntaxTriviaList trailingTrivia, SyntaxTriviaList leadingTrivia)
        {
            if (leadingTrivia.Any()
                && leadingTrivia.All(f => f.IsWhitespaceTrivia()))
            {
                SyntaxTriviaList.Enumerator en = trailingTrivia.GetEnumerator();

                while (en.MoveNext())
                {
                    SyntaxKind kind = en.Current.Kind();

                    if (kind == SyntaxKind.WhitespaceTrivia)
                        continue;

                    if (kind == SyntaxKind.EndOfLineTrivia
                        && !en.MoveNext())
                    {
                        return true;
                    }

                    break;
                }
            }

            return false;
        }
    }
}
