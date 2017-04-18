// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatEmbeddedStatementOnSeparateLineRefactoring
    {
        internal static void Analyze(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement)
        {
            Analyze(context, ifStatement.CloseParenToken, ifStatement.Statement);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, CommonForEachStatementSyntax forEachStatement)
        {
            Analyze(context, forEachStatement.CloseParenToken, forEachStatement.Statement);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, ForStatementSyntax forStatement)
        {
            Analyze(context, forStatement.CloseParenToken, forStatement.Statement);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, UsingStatementSyntax usingStatement)
        {
            Analyze(context, usingStatement.CloseParenToken, usingStatement.Statement);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, WhileStatementSyntax whileStatement)
        {
            Analyze(context, whileStatement.CloseParenToken, whileStatement.Statement);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, DoStatementSyntax doStatement)
        {
            Analyze(context, doStatement.DoKeyword, doStatement.Statement);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, LockStatementSyntax lockStatement)
        {
            Analyze(context, lockStatement.CloseParenToken, lockStatement.Statement);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, FixedStatementSyntax fixedStatement)
        {
            Analyze(context, fixedStatement.CloseParenToken, fixedStatement.Statement);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxToken token, StatementSyntax statement)
        {
            if (!token.IsKind(SyntaxKind.None)
                && !token.IsMissing
                && statement?.IsKind(SyntaxKind.Block, SyntaxKind.EmptyStatement) == false
                && context.SyntaxTree().IsSingleLineSpan(TextSpan.FromBounds(token.SpanStart, statement.SpanStart)))
            {
                ReportDiagnostic(context, statement);
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, ElseClauseSyntax elseClause)
        {
            StatementSyntax statement = elseClause.Statement;

            if (statement?.IsKind(SyntaxKind.Block, SyntaxKind.IfStatement) == false
                && context.SyntaxTree().IsSingleLineSpan(TextSpan.FromBounds(elseClause.ElseKeyword.SpanStart, statement.SpanStart)))
            {
                ReportDiagnostic(context, statement);
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, StatementSyntax statement)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.FormatEmbeddedStatementOnSeparateLine,
                statement);
        }
    }
}
