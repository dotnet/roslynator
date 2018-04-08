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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeIfStatementsRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, StatementListSelection selectedStatements)
        {
            int count = 0;

            for (int i = 0; i < selectedStatements.Count; i++)
            {
                if (!(selectedStatements[i] is IfStatementSyntax))
                    return;

                count++;
            }

            if (count <= 1)
                return;

            context.RegisterRefactoring(
                "Merge if statements",
                cancellationToken => RefactorAsync(context.Document, selectedStatements, cancellationToken));
        }

        public static Task<Document> RefactorAsync(
            Document document,
            StatementListSelection selectedStatements,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ImmutableArray<IfStatementSyntax> ifStatements = selectedStatements.Cast<IfStatementSyntax>().ToImmutableArray();

            IfStatementSyntax newIfStatement = IfStatement(
                BinaryExpression(SyntaxKind.LogicalOrExpression, ifStatements.Select(f => f.Condition)),
                Block(CreateStatements(ifStatements)));

            SyntaxList<StatementSyntax> statements = selectedStatements.UnderlyingList;

            int index = statements.IndexOf(ifStatements[0]);

            SyntaxList<StatementSyntax> newStatements = statements.Replace(
                ifStatements[0],
                newIfStatement
                    .WithLeadingTrivia(ifStatements[0].GetLeadingTrivia())
                    .WithTrailingTrivia(ifStatements[ifStatements.Length - 1].GetTrailingTrivia()));

            for (int i = 1; i < ifStatements.Length; i++)
                newStatements = newStatements.RemoveAt(index + 1);

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

        private static List<StatementSyntax> CreateStatements(ImmutableArray<IfStatementSyntax> ifStatements)
        {
            var newStatements = new List<StatementSyntax>();

            List<StatementSyntax> previousStatements = null;

            foreach (IfStatementSyntax ifStatement in ifStatements)
            {
                List<StatementSyntax> statements = GetStatementsFromIfStatement(ifStatement);

                if (previousStatements == null
                    || !AreEquivalent(statements, previousStatements))
                {
                    newStatements.AddRange(statements);
                }

                previousStatements = statements;
            }

            return newStatements;
        }

        private static List<StatementSyntax> GetStatementsFromIfStatement(IfStatementSyntax ifStatement)
        {
            if (ifStatement.Statement.IsKind(SyntaxKind.Block))
            {
                return ((BlockSyntax)ifStatement.Statement).Statements.ToList();
            }
            else
            {
                return new List<StatementSyntax>() { ifStatement.Statement };
            }
        }
    }
}
