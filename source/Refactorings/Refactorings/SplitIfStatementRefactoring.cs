// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SplitIfStatementRefactoring
    {
        internal static void ComputeRefactoring(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (ifStatement.IsSimpleIf())
            {
                StatementSyntax statement = ifStatement.Statement;

                if (statement?.IsMissing == false)
                {
                    ExpressionSyntax condition = ifStatement.Condition;

                    if (condition?.Kind() == SyntaxKind.LogicalOrExpression)
                    {
                        context.RegisterRefactoring(
                            "Split if",
                            cancellationToken => RefactorAsync(context.Document, ifStatement, cancellationToken));
                    }
                }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            StatementSyntax statement = ifStatement.Statement.WithoutTrivia();
            ExpressionSyntax condition = ifStatement.Condition;

            BinaryExpressionChainInfo info = SyntaxInfo.BinaryExpressionChainInfo((BinaryExpressionSyntax)condition);

            var ifStatements = new List<IfStatementSyntax>();

            foreach (ExpressionSyntax expression in info.Expressions)
                ifStatements.Add(SyntaxFactory.IfStatement(expression.TrimTrivia(), statement).WithFormatterAnnotation());

            ifStatements[0] = ifStatements[0].WithLeadingTrivia(ifStatement.GetLeadingTrivia());
            ifStatements[ifStatements.Count - 1] = ifStatements[ifStatements.Count - 1].WithTrailingTrivia(ifStatement.GetTrailingTrivia());

            if (ifStatement.IsEmbedded())
            {
                BlockSyntax block = SyntaxFactory.Block(ifStatements);

                return document.ReplaceNodeAsync(ifStatement, block, cancellationToken);
            }
            else
            {
                return document.ReplaceNodeAsync(ifStatement, ifStatements, cancellationToken);
            }
        }
    }
}
