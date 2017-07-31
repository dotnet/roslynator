// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeIfStatementWithNestedIfStatementRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement)
        {
            if (ifStatement.IsSimpleIf()
                && CheckCondition(ifStatement.Condition))
            {
                IfStatementSyntax nestedIf = GetNestedIfStatement(ifStatement);

                if (nestedIf != null
                    && nestedIf.Else == null
                    && CheckCondition(nestedIf.Condition)
                    && CheckTrivia(ifStatement, nestedIf)
                    && !ifStatement.SpanContainsDirectives())
                {
                    ReportDiagnostic(context, ifStatement, nestedIf);
                }
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement, IfStatementSyntax nestedIf)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatement, ifStatement);

            context.ReportToken(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut, nestedIf.IfKeyword);
            context.ReportToken(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut, nestedIf.OpenParenToken);
            context.ReportToken(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut, nestedIf.CloseParenToken);

            if (ifStatement.Statement.IsKind(SyntaxKind.Block)
                && nestedIf.Statement.IsKind(SyntaxKind.Block))
            {
                context.ReportBraces(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut, (BlockSyntax)nestedIf.Statement);
            }
        }

        private static bool CheckCondition(ExpressionSyntax condition)
        {
            return condition?.IsKind(SyntaxKind.LogicalOrExpression) == false;
        }

        private static bool CheckTrivia(IfStatementSyntax ifStatement, IfStatementSyntax nestedIf)
        {
            TextSpan span = TextSpan.FromBounds(
                nestedIf.FullSpan.Start,
                nestedIf.CloseParenToken.FullSpan.End);

            if (nestedIf.DescendantTrivia(span).All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                if (ifStatement.Statement.IsKind(SyntaxKind.Block)
                    && nestedIf.Statement.IsKind(SyntaxKind.Block))
                {
                    var block = (BlockSyntax)nestedIf.Statement;

                    return block.OpenBraceToken.LeadingTrivia.IsEmptyOrWhitespace()
                        && block.OpenBraceToken.TrailingTrivia.IsEmptyOrWhitespace()
                        && block.CloseBraceToken.LeadingTrivia.IsEmptyOrWhitespace()
                        && block.CloseBraceToken.TrailingTrivia.IsEmptyOrWhitespace();
                }

                return true;
            }

            return false;
        }

        private static IfStatementSyntax GetNestedIfStatement(IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = ifStatement.Statement;

            switch (statement?.Kind())
            {
                case SyntaxKind.Block:
                    {
                        SyntaxList<StatementSyntax> statements = ((BlockSyntax)statement).Statements;

                        if (statements.Count == 1
                            && statements[0].IsKind(SyntaxKind.IfStatement))
                        {
                            return (IfStatementSyntax)statements[0];
                        }

                        break;
                    }
                case SyntaxKind.IfStatement:
                    {
                        return (IfStatementSyntax)statement;
                    }
            }

            return null;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IfStatementSyntax nestedIf = GetNestedIfStatement(ifStatement);

            ExpressionSyntax left = ifStatement.Condition.Parenthesize();
            ExpressionSyntax right = nestedIf.Condition;

            if (!right.IsKind(SyntaxKind.LogicalAndExpression))
                right = right.Parenthesize();

            BinaryExpressionSyntax newCondition = CSharpFactory.LogicalAndExpression(left, right);

            IfStatementSyntax newNode = GetNewIfStatement(ifStatement, nestedIf)
                .WithCondition(newCondition)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(ifStatement, newNode, cancellationToken);
        }

        private static IfStatementSyntax GetNewIfStatement(IfStatementSyntax ifStatement, IfStatementSyntax ifStatement2)
        {
            if (ifStatement.Statement.IsKind(SyntaxKind.Block))
            {
                if (ifStatement2.Statement.IsKind(SyntaxKind.Block))
                {
                    return ifStatement.ReplaceNode(ifStatement2, ((BlockSyntax)ifStatement2.Statement).Statements);
                }
                else
                {
                    return ifStatement.ReplaceNode(ifStatement2, ifStatement2.Statement);
                }
            }
            else
            {
                return ifStatement.ReplaceNode(ifStatement.Statement, ifStatement2.Statement);
            }
        }
    }
}
