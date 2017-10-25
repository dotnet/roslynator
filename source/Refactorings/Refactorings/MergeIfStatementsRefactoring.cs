// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeIfStatementsRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, StatementsSelection selectedStatements)
        {
            List<IfStatementSyntax> ifStatements = GetIfStatements(selectedStatements);

            if (ifStatements?.Count > 1)
            {
                context.RegisterRefactoring(
                    "Merge if statements",
                    cancellationToken =>
                    {
                        return RefactorAsync(
                            context.Document,
                            selectedStatements.Info,
                            ifStatements.ToImmutableArray(),
                            cancellationToken);
                    });
            }
        }

        private static List<IfStatementSyntax> GetIfStatements(IEnumerable<StatementSyntax> statements)
        {
            List<IfStatementSyntax> ifStatements = null;

            using (IEnumerator<StatementSyntax> en = statements.GetEnumerator())
            {
                while (en.MoveNext())
                {
                    if (en.Current.IsKind(SyntaxKind.IfStatement))
                    {
                        (ifStatements ?? (ifStatements = new List<IfStatementSyntax>())).Add((IfStatementSyntax)en.Current);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return ifStatements;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            StatementsInfo statementsInfo,
            ImmutableArray<IfStatementSyntax> ifStatements,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IfStatementSyntax newIfStatement = IfStatement(
                CreateCondition(ifStatements),
                Block(CreateStatements(ifStatements)));

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            int index = statements.IndexOf(ifStatements[0]);

            SyntaxList<StatementSyntax> newStatements = statements.Replace(
                ifStatements[0],
                newIfStatement
                    .WithLeadingTrivia(ifStatements[0].GetLeadingTrivia())
                    .WithTrailingTrivia(ifStatements[ifStatements.Length - 1].GetTrailingTrivia()));

            for (int i = 1; i < ifStatements.Length; i++)
                newStatements = newStatements.RemoveAt(index + 1);

            return document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken);
        }

        private static BinaryExpressionSyntax CreateCondition(ImmutableArray<IfStatementSyntax> ifStatements)
        {
            BinaryExpressionSyntax condition = LogicalOrExpression(
                AddParenthesesIfNecessary(ifStatements[0].Condition),
                AddParenthesesIfNecessary(ifStatements[1].Condition));

            for (int i = 2; i < ifStatements.Length; i++)
                condition = LogicalOrExpression(condition, AddParenthesesIfNecessary(ifStatements[i].Condition));

            return condition;
        }

        private static ExpressionSyntax AddParenthesesIfNecessary(ExpressionSyntax expression)
        {
            if (expression.IsKind(SyntaxKind.ConditionalExpression))
                return ParenthesizedExpression(expression);

            return expression;
        }

        private static List<StatementSyntax> CreateStatements(ImmutableArray<IfStatementSyntax> ifStatements)
        {
            var newStatements = new List<StatementSyntax>();

            List<StatementSyntax> previousStatements = null;

            foreach (IfStatementSyntax ifStatement in ifStatements)
            {
                List<StatementSyntax> statements = GetStatementsFromIfStatement(ifStatement);

                if (previousStatements == null
                    || !AreStatementsEquivalent(statements, previousStatements))
                {
                    newStatements.AddRange(statements);
                }

                previousStatements = statements;
            }

            return newStatements;
        }

        private static bool AreStatementsEquivalent(List<StatementSyntax> first, List<StatementSyntax> second)
        {
            if (first.Count == second.Count)
            {
                for (int i = 0; i < first.Count; i++)
                {
                    if (!SyntaxComparer.AreEquivalent(first[i], second[i]))
                        return false;
                }

                return true;
            }

            return false;
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
