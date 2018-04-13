// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SimplifyCodeBranchingAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.SimplifyCodeBranching); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        internal static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            if (ifStatement.Condition?.WalkDownParentheses().IsMissing != false)
                return;

            StatementSyntax statement = ifStatement.Statement;

            if (statement == null)
                return;

            var block = statement as BlockSyntax;

            if (block?.Statements.Any() == false
                && block.OpenBraceToken.TrailingTrivia.IsEmptyOrWhitespace()
                && block.CloseBraceToken.LeadingTrivia.IsEmptyOrWhitespace())
            {
                if (IsFixableIfElse(ifStatement))
                    context.ReportDiagnostic(DiagnosticDescriptors.SimplifyCodeBranching, ifStatement);
            }
            else
            {
                ElseClauseSyntax elseClause = ifStatement.Else;

                if (elseClause != null)
                {
                    if (IsFixableIfElseInsideWhile(ifStatement, elseClause))
                        context.ReportDiagnostic(DiagnosticDescriptors.SimplifyCodeBranching, ifStatement);
                }
                else if (IsFixableIfInsideWhileOrDo(ifStatement))
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.SimplifyCodeBranching, ifStatement);
                }
            }
        }

        private static bool IsFixableIfElse(IfStatementSyntax ifStatement)
        {
            if (ifStatement.SpanContainsDirectives())
                return false;

            ElseClauseSyntax elseClause = ifStatement.Else;

            if (elseClause == null)
                return false;

            StatementSyntax whenFalse = elseClause.Statement;

            if (whenFalse == null)
                return false;

            SyntaxKind kind = whenFalse.Kind();

            if (kind == SyntaxKind.IfStatement)
            {
                var nestedIf = (IfStatementSyntax)whenFalse;

                if (nestedIf.Condition?.WalkDownParentheses().IsMissing != false)
                    return false;

                StatementSyntax statement = nestedIf.Statement;

                if (statement == null)
                    return false;

                if ((statement as BlockSyntax)?.Statements.Any() == false)
                    return false;
            }
            else if (kind == SyntaxKind.Block)
            {
                if (!((BlockSyntax)whenFalse).Statements.Any())
                    return false;
            }

            return true;
        }

        private static bool IsFixableIfElseInsideWhile(
            IfStatementSyntax ifStatement,
            ElseClauseSyntax elseClause)
        {
            if (elseClause.SingleNonBlockStatementOrDefault()?.Kind() != SyntaxKind.BreakStatement)
                return false;

            SyntaxNode parent = ifStatement.Parent;

            if (parent is BlockSyntax block)
            {
                if (block.Statements.Count != 1)
                    return false;

                parent = block.Parent;
            }

            if (!(parent is WhileStatementSyntax whileStatement))
                return false;

            if (whileStatement.SpanContainsDirectives())
                return false;

            if (whileStatement.Condition?.WalkDownParentheses().Kind() != SyntaxKind.TrueLiteralExpression)
                return false;

            return true;
        }

        private static bool IsFixableIfInsideWhileOrDo(IfStatementSyntax ifStatement)
        {
            SyntaxNode parent = ifStatement.Parent;

            if (parent.Kind() != SyntaxKind.Block)
                return false;

            if (ifStatement.SingleNonBlockStatementOrDefault()?.Kind() != SyntaxKind.BreakStatement)
                return false;

            var block = (BlockSyntax)parent;

            SyntaxList<StatementSyntax> statements = block.Statements;

            int count = statements.Count;

            if (count == 1)
                return false;

            int index = statements.IndexOf(ifStatement);

            if (index != 0
                && index != count - 1)
            {
                return false;
            }

            parent = block.Parent;

            SyntaxKind kind = parent.Kind();

            if (kind == SyntaxKind.WhileStatement)
            {
                var whileStatement = (WhileStatementSyntax)parent;

                if (whileStatement.SpanContainsDirectives())
                    return false;

                if (whileStatement.Condition?.WalkDownParentheses().Kind() != SyntaxKind.TrueLiteralExpression)
                    return false;
            }
            else if (kind == SyntaxKind.DoStatement)
            {
                var doStatement = (DoStatementSyntax)parent;

                if (doStatement.SpanContainsDirectives())
                    return false;

                if (doStatement.Condition?.WalkDownParentheses().Kind() != SyntaxKind.TrueLiteralExpression)
                    return false;
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
