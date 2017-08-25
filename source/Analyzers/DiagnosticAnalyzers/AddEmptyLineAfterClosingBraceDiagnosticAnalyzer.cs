// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddEmptyLineAfterClosingBraceDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddEmptyLineAfterClosingBrace); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeBlock, SyntaxKind.Block);
            context.RegisterSyntaxNodeAction(AnalyzeWhileStatement, SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForStatement, SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForEachStatement, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(AnalyzeUsingStatement, SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(AnalyzeFixedStatement, SyntaxKind.FixedStatement);
            context.RegisterSyntaxNodeAction(AnalyzeCheckedStatement, SyntaxKind.CheckedStatement);
            context.RegisterSyntaxNodeAction(AnalyzeCheckedStatement, SyntaxKind.UncheckedStatement);
            context.RegisterSyntaxNodeAction(AnalyzeUnsafeStatement, SyntaxKind.UnsafeStatement);
            context.RegisterSyntaxNodeAction(AnalyzeLockStatement, SyntaxKind.LockStatement);
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeAction(AnalyzeTryStatement, SyntaxKind.TryStatement);
        }

        private void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;

            if (block.IsParentKind(SyntaxKind.Block))
                AnalyzeStatement(context, block, block);
        }

        private void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            var whileStatement = (WhileStatementSyntax)context.Node;

            if (whileStatement.IsParentKind(SyntaxKind.Block))
            {
                StatementSyntax statement = whileStatement.Statement;

                if (statement?.IsKind(SyntaxKind.Block) == true)
                    AnalyzeStatement(context, whileStatement, (BlockSyntax)statement);
            }
        }

        private void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
        {
            var forStatement = (ForStatementSyntax)context.Node;

            if (forStatement.IsParentKind(SyntaxKind.Block))
            {
                StatementSyntax statement = forStatement.Statement;

                if (statement?.IsKind(SyntaxKind.Block) == true)
                    AnalyzeStatement(context, forStatement, (BlockSyntax)statement);
            }
        }

        private void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
        {
            var forEachStatement = (ForEachStatementSyntax)context.Node;

            if (forEachStatement.IsParentKind(SyntaxKind.Block))
            {
                StatementSyntax statement = forEachStatement.Statement;

                if (statement?.IsKind(SyntaxKind.Block) == true)
                    AnalyzeStatement(context, forEachStatement, (BlockSyntax)statement);
            }
        }

        private void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
        {
            var usingStatement = (UsingStatementSyntax)context.Node;

            if (usingStatement.IsParentKind(SyntaxKind.Block))
            {
                StatementSyntax statement = usingStatement.Statement;

                if (statement?.IsKind(SyntaxKind.Block) == true)
                    AnalyzeStatement(context, usingStatement, (BlockSyntax)statement);
            }
        }

        private void AnalyzeFixedStatement(SyntaxNodeAnalysisContext context)
        {
            var fixedStatement = (FixedStatementSyntax)context.Node;

            if (fixedStatement.IsParentKind(SyntaxKind.Block))
            {
                StatementSyntax statement = fixedStatement.Statement;

                if (statement?.IsKind(SyntaxKind.Block) == true)
                    AnalyzeStatement(context, fixedStatement, (BlockSyntax)statement);
            }
        }

        private void AnalyzeCheckedStatement(SyntaxNodeAnalysisContext context)
        {
            var checkedStatement = (CheckedStatementSyntax)context.Node;

            if (checkedStatement.IsParentKind(SyntaxKind.Block))
                AnalyzeStatement(context, checkedStatement, checkedStatement.Block);
        }

        private void AnalyzeUnsafeStatement(SyntaxNodeAnalysisContext context)
        {
            var unsafeStatement = (UnsafeStatementSyntax)context.Node;

            if (unsafeStatement.IsParentKind(SyntaxKind.Block))
                AnalyzeStatement(context, unsafeStatement, unsafeStatement.Block);
        }

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            var lockStatement = (LockStatementSyntax)context.Node;

            if (lockStatement.IsParentKind(SyntaxKind.Block))
            {
                StatementSyntax statement = lockStatement.Statement;

                if (statement?.IsKind(SyntaxKind.Block) == true)
                    AnalyzeStatement(context, lockStatement, (BlockSyntax)statement);
            }
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            if (ifStatement.IsParentKind(SyntaxKind.Block))
            {
                StatementSyntax statement = ifStatement.Statement;

                if (statement?.IsKind(SyntaxKind.Block) == true)
                    AnalyzeStatement(context, ifStatement, (BlockSyntax)statement);
            }
        }

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;

            if (switchStatement.IsParentKind(SyntaxKind.Block))
                AnalyzeStatement(context, switchStatement, switchStatement.OpenBraceToken, switchStatement.CloseBraceToken);
        }

        private void AnalyzeTryStatement(SyntaxNodeAnalysisContext context)
        {
            var tryStatement = (TryStatementSyntax)context.Node;

            if (tryStatement.IsParentKind(SyntaxKind.Block))
            {
                FinallyClauseSyntax finallyClause = tryStatement.Finally;

                if (finallyClause != null)
                {
                    AnalyzeStatement(context, tryStatement, finallyClause.Block);
                }
                else
                {
                    CatchClauseSyntax catchClause = tryStatement.Catches.LastOrDefault();

                    if (catchClause != null)
                        AnalyzeStatement(context, tryStatement, catchClause.Block);
                }
            }
        }

        private void AnalyzeStatement(SyntaxNodeAnalysisContext context, StatementSyntax statement, BlockSyntax block)
        {
            if (block?.IsMissing == false)
                AnalyzeStatement(context, statement, block.OpenBraceToken, block.CloseBraceToken);
        }

        private void AnalyzeStatement(SyntaxNodeAnalysisContext context, StatementSyntax statement, SyntaxToken openBrace, SyntaxToken closeBrace)
        {
            var block = (BlockSyntax)statement.Parent;

            SyntaxList<StatementSyntax> statements = block.Statements;

            int index = statements.IndexOf(statement);

            Debug.Assert(index != -1, "");

            if (index != -1
                && index < statements.Count - 1)
            {
                int startLine = openBrace.GetSpanStartLine();

                int endLine = closeBrace.GetSpanEndLine();

                if (startLine < endLine)
                {
                    StatementSyntax nextStatement = statements[index + 1];

                    if (nextStatement.GetSpanStartLine() - endLine == 1)
                    {
                        SyntaxTrivia trivia = closeBrace
                            .TrailingTrivia
                            .FirstOrDefault(f => f.IsEndOfLineTrivia());

                        if (trivia.IsEndOfLineTrivia())
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.AddEmptyLineAfterClosingBrace,
                                trivia);
                        }
                    }
                }
            }
        }
    }
}
