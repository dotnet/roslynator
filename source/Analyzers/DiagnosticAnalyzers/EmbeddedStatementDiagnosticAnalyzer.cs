// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EmbeddedStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AddBraces,
                    DiagnosticDescriptors.FormatEmbeddedStatementOnSeparateLine,
                    DiagnosticDescriptors.AddEmptyLineAfterEmbeddedStatement);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeStatement(f),
                SyntaxKind.IfStatement,
                SyntaxKind.ForEachStatement,
                SyntaxKind.ForStatement,
                SyntaxKind.UsingStatement,
                SyntaxKind.WhileStatement,
                SyntaxKind.DoStatement,
                SyntaxKind.LockStatement,
                SyntaxKind.FixedStatement);
        }

        private void AnalyzeStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            AnalyzeEmbeddedStatement(context);

            if (context.Node.IsKind(SyntaxKind.IfStatement)
                && !IfElseAnalysis.IsIsolatedIf((IfStatementSyntax)context.Node))
            {
                return;
            }

            StatementSyntax statement = EmbeddedStatementAnalysis.GetEmbeddedStatementThatShouldBeInsideBlock(context.Node);

            if (statement != null)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddBraces,
                    statement.GetLocation(),
                    SyntaxHelper.GetNodeTitle(context.Node));
            }
        }

        private static void AnalyzeEmbeddedStatement(SyntaxNodeAnalysisContext context)
        {
            SyntaxToken token = GetToken(context.Node);

            if (token.IsMissing)
                return;

            StatementSyntax statement = GetStatement(context.Node);

            if (statement.IsKind(SyntaxKind.Block))
                return;

            if (statement.IsKind(SyntaxKind.EmptyStatement))
                return;

            if (token.GetSpanStartLine() == statement.GetSpanStartLine())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.FormatEmbeddedStatementOnSeparateLine,
                    statement.GetLocation());
            }
            else
            {
                var parentStatement = (StatementSyntax)context.Node;

                var block = parentStatement.Parent as BlockSyntax;
                if (block == null)
                    return;

                int index = block.Statements.IndexOf(parentStatement);

                if (index == block.Statements.Count - 1)
                    return;

                int diff = block.Statements[index + 1].GetSpanStartLine() - statement.GetSpanEndLine();

                if (diff < 2)
                {
                    SyntaxTrivia trivia = statement
                        .GetTrailingTrivia()
                        .FirstOrDefault(f => f.IsEndOfLineTrivia());

                    if (trivia.IsEndOfLineTrivia())
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.AddEmptyLineAfterEmbeddedStatement,
                            trivia.GetLocation());
                    }
                }
            }
        }

        private static SyntaxToken GetToken(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.WhileStatement:
                    return ((WhileStatementSyntax)node).CloseParenToken;
                case SyntaxKind.DoStatement:
                    return ((DoStatementSyntax)node).DoKeyword;
                case SyntaxKind.ForStatement:
                    return ((ForStatementSyntax)node).CloseParenToken;
                case SyntaxKind.ForEachStatement:
                    return ((ForEachStatementSyntax)node).CloseParenToken;
                case SyntaxKind.UsingStatement:
                    return ((UsingStatementSyntax)node).CloseParenToken;
                case SyntaxKind.FixedStatement:
                    return ((FixedStatementSyntax)node).CloseParenToken;
                case SyntaxKind.LockStatement:
                    return ((LockStatementSyntax)node).CloseParenToken;
                case SyntaxKind.IfStatement:
                    return ((IfStatementSyntax)node).CloseParenToken;
                default:
                    return SyntaxFactory.Token(SyntaxKind.None);
            }
        }

        private static StatementSyntax GetStatement(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.WhileStatement:
                    return ((WhileStatementSyntax)node).Statement;
                case SyntaxKind.DoStatement:
                    return ((DoStatementSyntax)node).Statement;
                case SyntaxKind.ForStatement:
                    return ((ForStatementSyntax)node).Statement;
                case SyntaxKind.ForEachStatement:
                    return ((ForEachStatementSyntax)node).Statement;
                case SyntaxKind.UsingStatement:
                    return ((UsingStatementSyntax)node).Statement;
                case SyntaxKind.FixedStatement:
                    return ((FixedStatementSyntax)node).Statement;
                case SyntaxKind.LockStatement:
                    return ((LockStatementSyntax)node).Statement;
                case SyntaxKind.IfStatement:
                    return ((IfStatementSyntax)node).Statement;
                default:
                    return null;
            }
        }
    }
}
