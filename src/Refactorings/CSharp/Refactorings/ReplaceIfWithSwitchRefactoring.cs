// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.CSharpFacts;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceIfWithSwitchRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (!ifStatement.IsTopmostIf())
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
            {
                if (ifOrElse.IsIf)
                {
                    if (!CanRefactor(ifOrElse.AsIf(), semanticModel, context.CancellationToken))
                        return;
                }
                else if (ContainsBreakStatementThatBelongsToParentLoop(ifOrElse.AsElse().Statement))
                {
                    return;
                }
            }

            context.RegisterRefactoring(
                (ifStatement.IsSimpleIf()) ? "Replace if with switch" : "Replace if-else with switch",
                cancellationToken => RefactorAsync(context.Document, ifStatement, cancellationToken));
        }

        private static bool CanRefactor(
            IfStatementSyntax ifStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = ifStatement.Condition?.WalkDownParentheses();

            return condition.IsKind(SyntaxKind.EqualsExpression, SyntaxKind.LogicalOrExpression)
                && IsFixableCondition((BinaryExpressionSyntax)condition, null, semanticModel, cancellationToken)
                && !ContainsBreakStatementThatBelongsToParentLoop(ifStatement.Statement);
        }

        private static bool IsFixableCondition(
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax switchExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            bool success = true;

            while (success)
            {
                success = false;

                SyntaxKind kind = binaryExpression.Kind();

                if (kind == SyntaxKind.LogicalOrExpression)
                {
                    ExpressionSyntax right = binaryExpression.Right.WalkDownParentheses();

                    if (right?.Kind() == SyntaxKind.EqualsExpression)
                    {
                        var equalsExpression = (BinaryExpressionSyntax)right;

                        if (IsFixableEqualsExpression(equalsExpression, switchExpression, semanticModel, cancellationToken))
                        {
                            if (switchExpression == null)
                            {
                                switchExpression = equalsExpression.Left?.WalkDownParentheses();

                                if (!IsFixableSwitchExpression(switchExpression, semanticModel, cancellationToken))
                                    return false;
                            }

                            ExpressionSyntax left = binaryExpression.Left?.WalkDownParentheses();

                            if (left.IsKind(SyntaxKind.LogicalOrExpression, SyntaxKind.EqualsExpression))
                            {
                                binaryExpression = (BinaryExpressionSyntax)left;
                                success = true;
                            }
                        }
                    }
                }
                else if (kind == SyntaxKind.EqualsExpression)
                {
                    return IsFixableEqualsExpression(binaryExpression, switchExpression, semanticModel, cancellationToken);
                }
            }

            return false;
        }

        private static bool IsFixableEqualsExpression(
            BinaryExpressionSyntax equalsExpression,
            ExpressionSyntax switchExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = equalsExpression.Left?.WalkDownParentheses();

            if (IsFixableSwitchExpression(left, semanticModel, cancellationToken))
            {
                ExpressionSyntax right = equalsExpression.Right?.WalkDownParentheses();

                if (IsFixableSwitchExpression(right, semanticModel, cancellationToken)
                    && semanticModel.HasConstantValue(right))
                {
                    return switchExpression == null
                        || CSharpFactory.AreEquivalent(left, switchExpression);
                }
            }

            return false;
        }

        private static bool IsFixableSwitchExpression(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expression == null)
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(expression, cancellationToken).ConvertedType;

            if (typeSymbol == null)
                return false;

            return SymbolUtility.SupportsSwitchExpression(typeSymbol);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            SwitchStatementSyntax switchStatement = SwitchStatement(
                GetSwitchExpression(ifStatement),
                List(CreateSwitchSections(ifStatement)));

            switchStatement = switchStatement
                .WithTriviaFrom(ifStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(ifStatement, switchStatement, cancellationToken);
        }

        private static ExpressionSyntax GetSwitchExpression(IfStatementSyntax ifStatement)
        {
            var condition = (BinaryExpressionSyntax)ifStatement.Condition.WalkDownParentheses();

            if (condition.IsKind(SyntaxKind.LogicalOrExpression))
            {
                var right = (BinaryExpressionSyntax)condition.Right.WalkDownParentheses();

                return right.Left.WalkDownParentheses();
            }

            return condition.Left.WalkDownParentheses();
        }

        private static IEnumerable<SwitchSectionSyntax> CreateSwitchSections(IfStatementSyntax ifStatement)
        {
            foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
            {
                if (ifOrElse.IsIf)
                {
                    ifStatement = ifOrElse.AsIf();

                    var condition = ifStatement.Condition.WalkDownParentheses() as BinaryExpressionSyntax;

                    List<SwitchLabelSyntax> labels = CreateSwitchLabels(condition, new List<SwitchLabelSyntax>());
                    labels.Reverse();

                    SwitchSectionSyntax section = SwitchSection(
                        List(labels),
                        AddBreakStatementIfNecessary(ifStatement.Statement));

                    yield return section;
                }
                else
                {
                    yield return DefaultSwitchSection(AddBreakStatementIfNecessary(ifOrElse.Statement));
                }
            }
        }

        private static SyntaxList<StatementSyntax> AddBreakStatementIfNecessary(StatementSyntax statement)
        {
            if (statement.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement;

                SyntaxList<StatementSyntax> statements = block.Statements;

                if (statements.Any()
                    && IsJumpStatement(statements.Last().Kind()))
                {
                    return SingletonList<StatementSyntax>(block);
                }
                else
                {
                    return SingletonList<StatementSyntax>(block.AddStatements(BreakStatement()));
                }
            }
            else if (IsJumpStatement(statement.Kind()))
            {
                return SingletonList(statement);
            }
            else
            {
                return SingletonList<StatementSyntax>(Block(statement, BreakStatement()));
            }
        }

        private static List<SwitchLabelSyntax> CreateSwitchLabels(BinaryExpressionSyntax binaryExpression, List<SwitchLabelSyntax> labels)
        {
            if (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
            {
                labels.Add(CaseSwitchLabel(binaryExpression.Right.WalkDownParentheses()));
            }
            else
            {
                var equalsExpression = (BinaryExpressionSyntax)binaryExpression.Right.WalkDownParentheses();

                labels.Add(CaseSwitchLabel(equalsExpression.Right.WalkDownParentheses()));

                if (binaryExpression.IsKind(SyntaxKind.LogicalOrExpression))
                    return CreateSwitchLabels((BinaryExpressionSyntax)binaryExpression.Left.WalkDownParentheses(), labels);
            }

            return labels;
        }

        private static bool ContainsBreakStatementThatBelongsToParentLoop(StatementSyntax statement)
        {
            if (ShouldCheckBreakStatement())
            {
                foreach (SyntaxNode descendant in statement.DescendantNodes(statement.Span, f => !IsLoopOrNestedMethod(f.Kind())))
                {
                    if (descendant.IsKind(SyntaxKind.BreakStatement))
                        return true;
                }
            }

            return false;

            bool IsLoopOrNestedMethod(SyntaxKind kind)
            {
                return IsIterationStatement(kind) || IsFunction(kind);
            }

            bool ShouldCheckBreakStatement()
            {
                for (SyntaxNode node = statement.Parent; node != null; node = node.Parent)
                {
                    if (node is MemberDeclarationSyntax)
                        break;

                    SyntaxKind kind = node.Kind();

                    if (IsFunction(kind))
                        break;

                    if (IsIterationStatement(kind))
                        return true;
                }

                return false;
            }
        }
    }
}