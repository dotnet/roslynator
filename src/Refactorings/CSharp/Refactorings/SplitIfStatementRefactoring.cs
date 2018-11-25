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

            if (simpleIf.Success)
            {
                if (simpleIf.Condition.IsKind(SyntaxKind.LogicalOrExpression))
                {
                    context.RegisterRefactoring(
                        "Split if",
                        ct => SplitSimpleIfAsync(context.Document, ifStatement, ct),
                        RefactoringIdentifiers.SplitIfStatement);
                }
            }
            else if (ifStatement.Parent.IsKind(SyntaxKind.ElseClause)
                    && ifStatement.Else == null
                    && ifStatement.Condition.IsKind(SyntaxKind.LogicalOrExpression))
            {
                context.RegisterRefactoring(
                    "Split if",
                    ct => SplitLastElseIfAsync(context.Document, ifStatement, ct),
                    RefactoringIdentifiers.SplitIfStatement);
            }
        }

        private static Task<Document> SplitSimpleIfAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = ifStatement.Condition;
            StatementSyntax statement = ifStatement.Statement.WithoutTrivia();

            List<IfStatementSyntax> ifStatements = SyntaxInfo.BinaryExpressionInfo(condition)
                .AsChain()
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

        private static Task<Document> SplitLastElseIfAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = ifStatement.Condition;
            StatementSyntax statement = ifStatement.Statement.WithoutTrivia();

            IfStatementSyntax newIfStatement = ifStatement;

            ElseClauseSyntax elseClause = null;

            ExpressionChain.Reversed.Enumerator en = SyntaxInfo.BinaryExpressionInfo(condition).AsChain().Reverse().GetEnumerator();

            while (en.MoveNext())
            {
                newIfStatement = IfStatement(en.Current.TrimTrivia(), statement, elseClause);

                elseClause = ElseClause(newIfStatement);
            }

            newIfStatement = newIfStatement
                .WithTriviaFrom(ifStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(ifStatement, newIfStatement, cancellationToken);
        }
    }
}
