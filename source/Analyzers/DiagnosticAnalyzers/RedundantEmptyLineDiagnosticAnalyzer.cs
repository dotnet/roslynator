// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantEmptyLineDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantEmptyLine); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeClassDeclaration(f), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeStructDeclaration(f), SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeInterfaceDeclaration(f), SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeNamespaceDeclaration(f), SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeSwitchStatement(f), SyntaxKind.SwitchStatement);
        }

        public void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (ClassDeclarationSyntax)context.Node;

            AnalyzeDeclaration(
                context,
                declaration.Members,
                declaration.OpenBraceToken,
                declaration.CloseBraceToken);
        }

        private void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (StructDeclarationSyntax)context.Node;

            AnalyzeDeclaration(
                context,
                declaration.Members,
                declaration.OpenBraceToken,
                declaration.CloseBraceToken);
        }

        private void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (InterfaceDeclarationSyntax)context.Node;

            AnalyzeDeclaration(
                context,
                declaration.Members,
                declaration.OpenBraceToken,
                declaration.CloseBraceToken);
        }

        private void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (NamespaceDeclarationSyntax)context.Node;

            AnalyzeNamespaceDeclaration(context, declaration);
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

        private static void AnalyzeNamespaceDeclaration(
            SyntaxNodeAnalysisContext context,
            NamespaceDeclarationSyntax declaration)
        {
            SyntaxList<MemberDeclarationSyntax> members = declaration.Members;
            SyntaxList<ExternAliasDirectiveSyntax> externs = declaration.Externs;

            if (externs.Any())
            {
                AnalyzeStart(context, externs.First(), declaration.OpenBraceToken);
            }
            else
            {
                SyntaxList<UsingDirectiveSyntax> usings = declaration.Usings;

                if (usings.Any())
                {
                    AnalyzeStart(context, usings.First(), declaration.OpenBraceToken);
                }
                else if (members.Any())
                {
                    AnalyzeStart(context, members.First(), declaration.OpenBraceToken);
                }
            }

            if (members.Any())
                AnalyzeEnd(context, members.Last(), declaration.CloseBraceToken);
        }

        public static void AnalyzeBlock(SyntaxNodeAnalysisContext context, BlockSyntax block)
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

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var switchStatement = (SwitchStatementSyntax)context.Node;

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
