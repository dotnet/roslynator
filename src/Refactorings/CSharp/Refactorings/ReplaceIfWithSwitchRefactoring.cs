// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.CSharpFacts;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceIfWithSwitchRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            IfStatementSyntax ifStatement,
            SemanticModel semanticModel)
        {
            if (!ifStatement.IsTopmostIf())
                return;

            ExpressionSyntax switchExpression = null;

            foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
            {
                if (ifOrElse.IsIf)
                {
                    IfStatementSyntax ifStatement2 = ifOrElse.AsIf();

                    (bool success, ExpressionSyntax switchExpression) result = Analyze(ifStatement2.Condition?.WalkDownParentheses(), switchExpression, semanticModel, context.CancellationToken);

                    if (!result.success)
                        return;

                    switchExpression = result.switchExpression;

                    if (ContainsBreakStatementThatBelongsToParentIterationStatement(ifStatement2.Statement))
                        return;
                }
                else if (ContainsBreakStatementThatBelongsToParentIterationStatement(ifOrElse.AsElse().Statement))
                {
                    return;
                }
            }

            Document document = context.Document;

            context.RegisterRefactoring(
                "Replace if with switch",
                ct => RefactorAsync(document, ifStatement, ct),
                RefactoringIdentifiers.ReplaceIfWithSwitch);
        }

        private static (bool success, ExpressionSyntax switchExpression) Analyze(
            ExpressionSyntax condition,
            ExpressionSyntax switchExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            switch (condition.Kind())
            {
                case SyntaxKind.LogicalOrExpression:
                    {
                        var logicalOrExpression = (BinaryExpressionSyntax)condition;

                        foreach (ExpressionSyntax expression in logicalOrExpression.AsChain())
                        {
                            ExpressionSyntax expression2 = expression.WalkDownParentheses();

                            if (!expression2.IsKind(SyntaxKind.EqualsExpression))
                                return default;

                            var equalsExpression = (BinaryExpressionSyntax)expression2;

                            if (!IsFixableEqualsExpression(equalsExpression))
                                return default;
                        }

                        break;
                    }
                case SyntaxKind.EqualsExpression:
                    {
                        var equalsExpression = (BinaryExpressionSyntax)condition;

                        if (!IsFixableEqualsExpression(equalsExpression))
                            return default;

                        break;
                    }
                case SyntaxKind.IsPatternExpression:
                    {
                        var isPatternExpression = (IsPatternExpressionSyntax)condition;

                        PatternSyntax pattern = isPatternExpression.Pattern;

                        Debug.Assert(pattern.IsKind(SyntaxKind.DeclarationPattern, SyntaxKind.ConstantPattern), pattern.Kind().ToString());

                        if (!pattern.IsKind(SyntaxKind.DeclarationPattern, SyntaxKind.ConstantPattern))
                            return default;

                        ExpressionSyntax expression = isPatternExpression.Expression.WalkDownParentheses();

                        if (switchExpression == null)
                        {
                            switchExpression = expression;
                        }
                        else if (!CSharpFactory.AreEquivalent(expression, switchExpression))
                        {
                            return default;
                        }

                        break;
                    }
                default:
                    {
                        return default;
                    }
            }

            return (true, switchExpression);

            bool IsFixableEqualsExpression(BinaryExpressionSyntax equalsExpression)
            {
                BinaryExpressionInfo binaryExpressionInfo = SyntaxInfo.BinaryExpressionInfo(equalsExpression);

                if (!binaryExpressionInfo.Success)
                    return false;

                ExpressionSyntax right = binaryExpressionInfo.Right;

                if (!right.IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultLiteralExpression, SyntaxKind.DefaultExpression))
                {
                    ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(right, cancellationToken).ConvertedType;

                    if (typeSymbol == null)
                        return false;

                    if (!SymbolUtility.SupportsSwitchExpression(typeSymbol))
                        return false;
                }

                if (!semanticModel.HasConstantValue(right, cancellationToken))
                    return false;

                ExpressionSyntax left = binaryExpressionInfo.Left;

                if (switchExpression == null)
                {
                    switchExpression = left;
                }
                else if (!CSharpFactory.AreEquivalent(left, switchExpression))
                {
                    return false;
                }

                return true;
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            SwitchStatementSyntax switchStatement = SwitchStatement(
                GetSwitchExpression(ifStatement).WalkDownParentheses(),
                List(CreateSwitchSections(ifStatement)));

            switchStatement = switchStatement
                .WithTriviaFrom(ifStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(ifStatement, switchStatement, cancellationToken);
        }

        private static ExpressionSyntax GetSwitchExpression(IfStatementSyntax ifStatement)
        {
            ExpressionSyntax expression = ifStatement.Condition.WalkDownParentheses();

            switch (expression.Kind())
            {
                case SyntaxKind.LogicalOrExpression:
                    {
                        var logicalOrExpression = (BinaryExpressionSyntax)expression;

                        var right = (BinaryExpressionSyntax)logicalOrExpression.Right.WalkDownParentheses();

                        return right.Left;
                    }
                case SyntaxKind.EqualsExpression:
                    {
                        var equalsExpression = (BinaryExpressionSyntax)expression;

                        return equalsExpression.Left;
                    }
                case SyntaxKind.IsPatternExpression:
                    {
                        var isPatternExpression = (IsPatternExpressionSyntax)expression;

                        return isPatternExpression.Expression;
                    }
                default:
                    {
                        throw new InvalidOperationException();
                    }
            }
        }

        private static IEnumerable<SwitchSectionSyntax> CreateSwitchSections(IfStatementSyntax ifStatement)
        {
            foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
            {
                if (ifOrElse.IsIf)
                {
                    IfStatementSyntax ifStatement2 = ifOrElse.AsIf();

                    yield return SwitchSection(
                        CreateSwitchLabels(ifStatement2),
                        AddBreakStatementIfNecessary(ifStatement2.Statement));
                }
                else
                {
                    yield return DefaultSwitchSection(AddBreakStatementIfNecessary(ifOrElse.Statement));
                }
            }
        }

        private static SyntaxList<SwitchLabelSyntax> CreateSwitchLabels(IfStatementSyntax ifStatement)
        {
            ExpressionSyntax expression = ifStatement.Condition.WalkDownParentheses();

            switch (expression.Kind())
            {
                case SyntaxKind.LogicalOrExpression:
                    {
                        var logicalOrExpression = (BinaryExpressionSyntax)expression;

                        return logicalOrExpression.AsChain()
                            .Select(exp =>
                            {
                                var binaryExpression = (BinaryExpressionSyntax)exp.WalkDownParentheses();
                                return CreateCaseSwitchLabel(binaryExpression.Right.WalkDownParentheses());
                            })
                            .ToSyntaxList();
                    }
                case SyntaxKind.EqualsExpression:
                    {
                        var equalsExpression = (BinaryExpressionSyntax)expression;

                        return SingletonList(CreateCaseSwitchLabel(equalsExpression.Right.WalkDownParentheses()));
                    }
                case SyntaxKind.IsPatternExpression:
                    {
                        var isPatternExpression = (IsPatternExpressionSyntax)expression;

                        PatternSyntax pattern = isPatternExpression.Pattern;

                        if (pattern is ConstantPatternSyntax constantPattern)
                        {
                            return SingletonList(CreateCaseSwitchLabel(constantPattern.Expression));
                        }
                        else if (pattern is DeclarationPatternSyntax)
                        {
                            return SingletonList<SwitchLabelSyntax>(CasePatternSwitchLabel(pattern, Token(SyntaxKind.ColonToken)));
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                default:
                    {
                        throw new InvalidOperationException();
                    }
            }
        }

        private static SwitchLabelSyntax CreateCaseSwitchLabel(ExpressionSyntax expression)
        {
            if (expression.IsKind(SyntaxKind.DefaultLiteralExpression))
                expression = NullLiteralExpression().WithTriviaFrom(expression);

            return CaseSwitchLabel(expression);
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

        private static bool ContainsBreakStatementThatBelongsToParentIterationStatement(StatementSyntax statement)
        {
            if (IsContainedInIterationStatement())
            {
                foreach (SyntaxNode node in statement.DescendantNodes(statement.Span, f => ShouldDescendIntoChildren(f.Kind())))
                {
                    if (node.IsKind(SyntaxKind.BreakStatement))
                        return true;
                }
            }

            return false;

            bool ShouldDescendIntoChildren(SyntaxKind kind)
            {
                return !IsIterationStatement(kind) && !IsFunction(kind);
            }

            bool IsContainedInIterationStatement()
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