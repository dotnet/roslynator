// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.CSharpTypeFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceSwitchWithIfElseRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, SwitchStatementSyntax switchStatement)
        {
            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            bool containsSectionWithoutDefault = false;

            foreach (SwitchSectionSyntax section in sections)
            {
                bool containsPattern = false;
                bool containsDefault = false;

                foreach (SwitchLabelSyntax label in section.Labels)
                {
                    switch (label.Kind())
                    {
                        case SyntaxKind.CasePatternSwitchLabel:
                            {
                                containsPattern = true;
                                break;
                            }
                        case SyntaxKind.CaseSwitchLabel:
                            {
                                break;
                            }
                        case SyntaxKind.DefaultSwitchLabel:
                            {
                                containsDefault = true;
                                break;
                            }
                        default:
                            {
                                Debug.Fail(label.Kind().ToString());
                                return;
                            }
                    }

                    if (containsDefault)
                    {
                        if (containsPattern)
                            return;
                    }
                    else
                    {
                        containsSectionWithoutDefault = true;
                    }
                }
            }

            if (!containsSectionWithoutDefault)
                return;

            context.RegisterRefactoring(
                (sections.Count == 1) ? "Replace switch with if" : "Replace switch with if-else",
                cancellationToken => RefactorAsync(context.Document, switchStatement, cancellationToken));
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;
            IfStatementSyntax ifStatement = null;
            ElseClauseSyntax elseClause = null;

            int defaultSectionIndex = sections.IndexOf(section => section.Labels.Contains(SyntaxKind.DefaultSwitchLabel));

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

                SemanticModel semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);

                IfStatementSyntax @if = IfStatement(
                    CreateCondition(switchStatement, sections[i], semanticModel),
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

            ifStatement = ifStatement
                .WithTriviaFrom(switchStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(switchStatement, ifStatement, cancellationToken).ConfigureAwait(false);
        }

        private static ExpressionSyntax CreateCondition(
            SwitchStatementSyntax switchStatement,
            SwitchSectionSyntax switchSection,
            SemanticModel semanticModel)
        {
            ExpressionSyntax condition = null;

            ExpressionSyntax switchExpression = switchStatement.Expression;

            SyntaxList<SwitchLabelSyntax> labels = switchSection.Labels;

            for (int i = labels.Count - 1; i >= 0; i--)
            {
                ExpressionSyntax expression;

                switch (labels[i])
                {
                    case CaseSwitchLabelSyntax constantLabel:
                        {
                            BinaryExpressionSyntax equalsExpression = EqualsExpression(switchExpression, constantLabel.Value);

                            if (semanticModel.GetSpeculativeMethodSymbol(switchSection.SpanStart, equalsExpression) != null)
                            {
                                expression = equalsExpression;
                            }
                            else
                            {
                                expression = SimpleMemberInvocationExpression(
                                    ObjectType(),
                                    IdentifierName("Equals"),
                                    ArgumentList(Argument(switchExpression), Argument(constantLabel.Value)));
                            }

                            break;
                        }
                    case CasePatternSwitchLabelSyntax patternLabel:
                        {
                            expression = IsPatternExpression(switchExpression.Parenthesize(), patternLabel.Pattern).Parenthesize();
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }

                if (condition != null)
                {
                    condition = LogicalOrExpression(expression, condition);
                }
                else
                {
                    condition = expression;
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
