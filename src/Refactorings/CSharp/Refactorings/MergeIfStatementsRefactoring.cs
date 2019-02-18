// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeIfStatementsRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, StatementListSelection selectedStatements)
        {
            if (selectedStatements.Count < 2)
                return;

            StatementSyntax statement = null;

            for (int i = 0; i < selectedStatements.Count; i++)
            {
                if (!(selectedStatements[i] is IfStatementSyntax ifStatement))
                    return;

                if (!ifStatement.IsSimpleIf())
                    return;

                ExpressionSyntax condition = ifStatement.Condition;

                if (condition?.IsMissing != false)
                    return;

                if (condition.WalkDownParentheses().IsKind(SyntaxKind.IsPatternExpression))
                    return;

                if (i == 0)
                {
                    statement = ifStatement.Statement;

                    if (statement == null)
                        return;
                }
                else if (!AreEquivalent(statement, ifStatement.Statement))
                {
                    return;
                }
            }

            Document document = context.Document;

            context.RegisterRefactoring(
                "Merge if statements",
                ct => RefactorAsync(document, selectedStatements, ct),
                RefactoringIdentifiers.MergeIfStatements);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            StatementListSelection selectedStatements,
            CancellationToken cancellationToken)
        {
            ImmutableArray<IfStatementSyntax> ifStatements = selectedStatements.Cast<IfStatementSyntax>().ToImmutableArray();

            SyntaxList<StatementSyntax> newStatements = selectedStatements.UnderlyingList.Replace(
                ifStatements[0],
                ifStatements[0]
                    .WithCondition(BinaryExpression(SyntaxKind.LogicalOrExpression, ifStatements.Select(f => f.Condition)))
                    .WithLeadingTrivia(ifStatements[0].GetLeadingTrivia())
                    .WithTrailingTrivia(ifStatements[ifStatements.Length - 1].GetTrailingTrivia()));

            newStatements = newStatements.RemoveRange(selectedStatements.FirstIndex + 1, selectedStatements.Count - 1);

            return document.ReplaceStatementsAsync(SyntaxInfo.StatementListInfo(selectedStatements), newStatements, cancellationToken);
        }

        private static BinaryExpressionSyntax BinaryExpression(SyntaxKind kind, IEnumerable<ExpressionSyntax> expressions)
        {
            if (expressions == null)
                throw new ArgumentNullException(nameof(expressions));

            using (IEnumerator<ExpressionSyntax> en = expressions.GetEnumerator())
            {
                if (!en.MoveNext())
                    throw new ArgumentException("Sequence cannot be empty.", nameof(expressions));

                ExpressionSyntax first = en.Current;

                if (!en.MoveNext())
                    throw new ArgumentException("Sequence must contain at least two elements.", nameof(expressions));

                BinaryExpressionSyntax binaryExpression = SyntaxFactory.BinaryExpression(
                    kind,
                    first.Parenthesize(),
                    en.Current.Parenthesize());

                while (en.MoveNext())
                    binaryExpression = SyntaxFactory.BinaryExpression(kind, binaryExpression.Parenthesize(), en.Current.Parenthesize());

                return binaryExpression;
            }
        }

        private static bool AreEquivalent(StatementSyntax statement1, StatementSyntax statement2)
        {
            if (statement1 == null)
                return false;

            if (statement2 == null)
                return false;

            if (!(statement1 is BlockSyntax block1))
                return CSharpFactory.AreEquivalent(statement1, statement2.SingleNonBlockStatementOrDefault());

            SyntaxList<StatementSyntax> statements1 = block1.Statements;

            if (!(statement2 is BlockSyntax block2))
                return CSharpFactory.AreEquivalent(statement2, statement1.SingleNonBlockStatementOrDefault());

            SyntaxList<StatementSyntax> statements2 = block2.Statements;

            return SyntaxFactory.AreEquivalent(statements1, statements2, topLevel: false);
        }
    }
}
