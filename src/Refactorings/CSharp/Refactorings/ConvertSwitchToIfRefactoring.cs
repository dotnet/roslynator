// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    internal static class ConvertSwitchToIfRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, SwitchStatementSyntax switchStatement)
        {
            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            var containsNonDefaultSection = false;
            var containsSectionWithoutDefault = false;

            foreach (SwitchSectionSyntax section in sections)
            {
                var containsPattern = false;
                var containsDefault = false;

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
                                SyntaxDebug.Fail(label);
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

                if (!containsDefault)
                    containsNonDefaultSection = true;
            }

            if (!containsNonDefaultSection)
                return;

            if (!containsSectionWithoutDefault)
                return;

            context.RegisterRefactoring(
                "Convert to 'if'",
                ct => ConvertSwitchToIfAsync(context.Document, switchStatement, ct),
                RefactoringDescriptors.ConvertSwitchToIf);
        }

        private static async Task<Document> ConvertSwitchToIfAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            CancellationToken cancellationToken = default)
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

                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

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

                            if (patternLabel.WhenClause != null)
                            {
                                expression = LogicalAndExpression(
                                    expression,
                                    patternLabel.WhenClause.Condition.Parenthesize())
                                    .Parenthesize();
                            }

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
