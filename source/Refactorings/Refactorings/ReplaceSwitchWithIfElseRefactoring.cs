// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceSwitchWithIfElseRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, SwitchStatementSyntax switchStatement)
        {
            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            if (sections.Any(section => !section.Labels.Contains(SyntaxKind.DefaultSwitchLabel)))
            {
                string title = (sections.Count == 1)
                    ? "Replace switch with if"
                    : "Replace switch with if-else";

                context.RegisterRefactoring(
                    title,
                    cancellationToken => RefactorAsync(context.Document, switchStatement, cancellationToken));
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IfStatementSyntax newNode = Refactor(switchStatement)
                .WithTriviaFrom(switchStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(switchStatement, newNode, cancellationToken);
        }

        private static IfStatementSyntax Refactor(SwitchStatementSyntax switchStatement)
        {
            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;
            IfStatementSyntax ifStatement = null;
            ElseClauseSyntax elseClause = null;

            int defaultSectionIndex = sections
                .IndexOf(section => section.Labels.Contains(SyntaxKind.DefaultSwitchLabel));

            if (defaultSectionIndex != -1)
            {
                SyntaxList<StatementSyntax> statements = GetSectionStatements(sections[defaultSectionIndex]);

                elseClause = ElseClause(Block(statements));
            }

            for (int i = sections.Count - 1; i >= 0; i--)
            {
                if (i == defaultSectionIndex)
                    continue;

                SyntaxList<StatementSyntax> statements = GetSectionStatements(sections[i]);

                IfStatementSyntax @if = IfStatement(
                    CreateCondition(switchStatement, sections[i]),
                    Block(statements));

                if (ifStatement != null)
                {
                    ifStatement = @if.WithElse(ElseClause(ifStatement));
                }
                else
                {
                    ifStatement = @if;

                    if (elseClause != null)
                        ifStatement = ifStatement.WithElse(elseClause);
                }
            }

            return ifStatement;
        }

        private static ExpressionSyntax CreateCondition(SwitchStatementSyntax switchStatement, SwitchSectionSyntax switchSection)
        {
            ExpressionSyntax condition = null;

            for (int i = switchSection.Labels.Count - 1; i >= 0; i--)
            {
                BinaryExpressionSyntax equalsExpression = EqualsExpression(
                    switchStatement.Expression,
                    ((CaseSwitchLabelSyntax)switchSection.Labels[i]).Value);

                if (condition != null)
                {
                    condition = LogicalOrExpression(
                        equalsExpression,
                        condition);
                }
                else
                {
                    condition = equalsExpression;
                }
            }

            return condition;
        }

        private static SyntaxList<StatementSyntax> GetSectionStatements(SwitchSectionSyntax switchSection)
        {
            SyntaxList<StatementSyntax> statements = switchSection.Statements;

            if (statements.Count == 1
                && statements[0].IsKind(SyntaxKind.Block))
            {
                statements = ((BlockSyntax)statements[0]).Statements;
            }

            if (statements.Any())
            {
                StatementSyntax last = statements.Last();

                if (last.IsKind(SyntaxKind.BreakStatement))
                    return statements.Remove(last);
            }

            return statements;
        }
    }
}
