// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Analyzers
{
    internal static class MergeIfStatementWithNestedIfStatementAnalyzer
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement)
        {
            if (IfElseAnalysis.IsIsolatedIf(ifStatement)
                && ConditionAllowsMerging(ifStatement.Condition))
            {
                IfStatementSyntax nestedIf = GetNestedIfStatement(ifStatement);

                if (nestedIf != null
                    && nestedIf.Else == null
                    && ConditionAllowsMerging(nestedIf.Condition)
                    && TriviaAllowsMerging(ifStatement, nestedIf))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.MergeIfStatementWithNestedIfStatement,
                        ifStatement.GetLocation());

                    FadeOut(context, ifStatement, nestedIf);
                }
            }
        }

        private static bool ConditionAllowsMerging(ExpressionSyntax condition)
        {
            return condition != null
                && !condition.IsMissing
                && !condition.IsKind(SyntaxKind.LogicalOrExpression);
        }

        private static bool TriviaAllowsMerging(IfStatementSyntax ifStatement, IfStatementSyntax nestedIf)
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

                    return block.OpenBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && block.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && block.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && block.CloseBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia());
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
                        var block = (BlockSyntax)statement;

                        if (block.Statements.Count == 1
                            && block.Statements[0].IsKind(SyntaxKind.IfStatement))
                        {
                            return (IfStatementSyntax)block.Statements[0];
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

        private static void FadeOut(
            SyntaxNodeAnalysisContext context,
            IfStatementSyntax ifStatement,
            IfStatementSyntax nestedIf)
        {
            context.FadeOutToken(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut, nestedIf.IfKeyword);
            context.FadeOutToken(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut, nestedIf.OpenParenToken);
            context.FadeOutToken(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut, nestedIf.CloseParenToken);

            if (ifStatement.Statement.IsKind(SyntaxKind.Block)
                && nestedIf.Statement.IsKind(SyntaxKind.Block))
            {
                context.FadeOutBraces(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut, (BlockSyntax)nestedIf.Statement);
            }
        }
    }
}
