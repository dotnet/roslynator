// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SplitIfStatementRefactoring
    {
        internal static void ComputeRefactoring(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            SimpleIfStatementInfo simpleIf = SyntaxInfo.SimpleIfStatementInfo(ifStatement);

            if (!simpleIf.Success)
                return;

            if (simpleIf.Condition.Kind() != SyntaxKind.LogicalOrExpression)
                return;

            context.RegisterRefactoring(
                "Split if",
                cancellationToken => RefactorAsync(context.Document, ifStatement, cancellationToken));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            StatementSyntax statement = ifStatement.Statement.WithoutTrivia();
            ExpressionSyntax condition = ifStatement.Condition;

            List<IfStatementSyntax> ifStatements = SyntaxInfo.BinaryExpressionInfo(condition)
                .Expressions(leftToRight: true)
                .Select(expression => IfStatement(expression.TrimTrivia(), statement).WithFormatterAnnotation())
                .ToList();

            ifStatements[0] = ifStatements[0].WithLeadingTrivia(ifStatement.GetLeadingTrivia());
            ifStatements[ifStatements.Count - 1] = ifStatements[ifStatements.Count - 1].WithTrailingTrivia(ifStatement.GetTrailingTrivia());

            if (ifStatement.IsEmbedded())
            {
                BlockSyntax block = Block(ifStatements);

                return document.ReplaceNodeAsync(ifStatement, block, cancellationToken);
            }
            else
            {
                return document.ReplaceNodeAsync(ifStatement, ifStatements, cancellationToken);
            }
        }
    }
}
