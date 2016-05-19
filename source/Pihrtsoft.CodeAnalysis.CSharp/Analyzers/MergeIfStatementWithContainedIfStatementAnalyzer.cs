// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Analyzers
{
    internal static class MergeIfStatementWithContainedIfStatementAnalyzer
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement)
        {
            if (IfElseChainAnalysis.IsIsolatedIf(ifStatement)
                && CheckCondition(ifStatement.Condition))
            {
                IfStatementSyntax ifStatement2 = GetContainedIfStatement(ifStatement);

                if (ifStatement2 != null
                    && ifStatement2.Else == null
                    && CheckCondition(ifStatement2.Condition))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.MergeIfStatementWithContainedIfStatement,
                        ifStatement.GetLocation());

                    FadeOut(context, ifStatement, ifStatement2);
                }
            }
        }

        private static bool CheckCondition(ExpressionSyntax condition)
        {
            return condition != null
                && !condition.IsMissing
                && !condition.IsKind(SyntaxKind.LogicalOrExpression);
        }

        private static IfStatementSyntax GetContainedIfStatement(IfStatementSyntax ifStatement)
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
            IfStatementSyntax ifStatement2)
        {
            DiagnosticHelper.FadeOutToken(context, ifStatement2.IfKeyword, DiagnosticDescriptors.MergeIfStatementWithContainedIfStatementFadeOut);
            DiagnosticHelper.FadeOutToken(context, ifStatement2.OpenParenToken, DiagnosticDescriptors.MergeIfStatementWithContainedIfStatementFadeOut);
            DiagnosticHelper.FadeOutToken(context, ifStatement2.CloseParenToken, DiagnosticDescriptors.MergeIfStatementWithContainedIfStatementFadeOut);

            if (ifStatement.Statement.IsKind(SyntaxKind.Block)
                && ifStatement2.Statement.IsKind(SyntaxKind.Block))
            {
                DiagnosticHelper.FadeOutBraces(context, (BlockSyntax)ifStatement2.Statement, DiagnosticDescriptors.MergeIfStatementWithContainedIfStatementFadeOut);
            }
        }
    }
}
