// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Roslynator.CSharp.Refactorings.If.IfRefactoringHelper;

namespace Roslynator.CSharp.Refactorings.If
{
    internal abstract class IfRefactoring
    {
        public static IfAnalysisOptions DefaultOptions { get; } = new IfAnalysisOptions();

        protected IfRefactoring(IfStatementSyntax ifStatement)
        {
            IfStatement = ifStatement;
        }

        public abstract RefactoringKind Kind { get; }

        public abstract string Title { get; }

        public abstract Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken);

        public IfStatementSyntax IfStatement { get; }

        public static ImmutableArray<IfRefactoring> Analyze(
            IfStatementSyntax ifStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Analyze(ifStatement, DefaultOptions, semanticModel, cancellationToken);
        }

        public static ImmutableArray<IfRefactoring> Analyze(
            IfStatementSyntax ifStatement,
            IfAnalysisOptions options,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IfElseChain.IsTopmostIf(ifStatement))
            {
                ExpressionSyntax condition = ifStatement.Condition;

                if (condition != null)
                {
                    ElseClauseSyntax elseClause = ifStatement.Else;

                    if (elseClause != null)
                    {
                        if (options.CheckSpanDirectives(ifStatement))
                        {
                            StatementSyntax statement1 = GetSingleStatementOrDefault(ifStatement);

                            if (statement1 != null)
                            {
                                StatementSyntax statement2 = GetSingleStatementOrDefault(elseClause);

                                if (statement2 != null)
                                {
                                    SyntaxKind kind = statement1.Kind();

                                    if (kind == statement2.Kind())
                                    {
                                        switch (kind)
                                        {
                                            case SyntaxKind.ExpressionStatement:
                                                {
                                                    return Analyze(ifStatement, condition, (ExpressionStatementSyntax)statement1, (ExpressionStatementSyntax)statement2, options);
                                                }
                                            case SyntaxKind.ReturnStatement:
                                                {
                                                    return Analyze(ifStatement, condition, ((ReturnStatementSyntax)statement1).Expression, ((ReturnStatementSyntax)statement2).Expression, semanticModel, cancellationToken, options, isYield: false);
                                                }
                                            case SyntaxKind.YieldReturnStatement:
                                                {
                                                    return Analyze(ifStatement, condition, ((YieldStatementSyntax)statement1).Expression, ((YieldStatementSyntax)statement2).Expression, semanticModel, cancellationToken, options, isYield: true);
                                                }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        StatementSyntax nextStatement = ifStatement.NextStatement();

                        if (nextStatement?.IsKind(SyntaxKind.ReturnStatement) == true)
                            return Analyze(ifStatement, (ReturnStatementSyntax)nextStatement, options, semanticModel, cancellationToken);
                    }
                }
            }

            return ImmutableArray<IfRefactoring>.Empty;
        }

        private static ImmutableArray<IfRefactoring> Analyze(
            IfStatementSyntax ifStatement,
            ExpressionSyntax condition,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            IfAnalysisOptions options,
            bool isYield)
        {
            if (expression1?.IsMissing == false
                && expression2?.IsMissing == false)
            {
                if (options.UseCoalesceExpression)
                {
                    SyntaxKind conditionKind = condition.Kind();

                    if (conditionKind == SyntaxKind.EqualsExpression)
                    {
                        var binaryExpression = (BinaryExpressionSyntax)condition;

                        if (IsNullLiteral(binaryExpression.Right)
                            && IsEquivalent(binaryExpression.Left, expression2))
                        {
                            return IfToReturnWithCoalesceExpression.Create(ifStatement, expression2, expression1, isYield).ToImmutableArray();
                        }
                    }
                    else if (conditionKind == SyntaxKind.NotEqualsExpression)
                    {
                        var binaryExpression = (BinaryExpressionSyntax)condition;

                        if (IsNullLiteral(binaryExpression.Right)
                            && IsEquivalent(binaryExpression.Left, expression1))
                        {
                            return IfToReturnWithCoalesceExpression.Create(ifStatement, expression1, expression2, isYield).ToImmutableArray();
                        }
                    }
                }

                IfToReturnWithBooleanExpression ifToReturnWithBooleanExpression = null;

                if (options.UseBooleanExpression
                    && semanticModel.GetTypeSymbol(expression1, cancellationToken)?.IsBoolean() == true
                    && semanticModel.GetTypeSymbol(expression2, cancellationToken)?.IsBoolean() == true)
                {
                    ifToReturnWithBooleanExpression = IfToReturnWithBooleanExpression.Create(ifStatement, expression1, expression2, isYield);
                }

                IfToReturnWithConditionalExpression ifToReturnWithConditionalExpression = null;

                if (options.UseConditionalExpression
                    && (!expression1.IsBooleanLiteralExpression() || !expression2.IsBooleanLiteralExpression()))
                {
                    ifToReturnWithConditionalExpression = IfToReturnWithConditionalExpression.Create(ifStatement, expression1, expression2, isYield);
                }

                return ToImmutableArray(ifToReturnWithBooleanExpression, ifToReturnWithConditionalExpression);
            }

            return ImmutableArray<IfRefactoring>.Empty;
        }

        private static ImmutableArray<IfRefactoring> Analyze(
            IfStatementSyntax ifStatement,
            ExpressionSyntax condition,
            ExpressionStatementSyntax expressionStatement1,
            ExpressionStatementSyntax expressionStatement2,
            IfAnalysisOptions options)
        {
            ExpressionSyntax expression1 = expressionStatement1.Expression;

            if (IsSimpleAssignment(expression1))
            {
                ExpressionSyntax expression2 = expressionStatement2.Expression;

                if (IsSimpleAssignment(expression2))
                {
                    var assignment1 = (AssignmentExpressionSyntax)expression1;
                    var assignment2 = (AssignmentExpressionSyntax)expression2;

                    ExpressionSyntax left1 = assignment1.Left;
                    ExpressionSyntax right1 = assignment1.Right;

                    if (left1?.IsMissing == false
                        && right1?.IsMissing == false)
                    {
                        ExpressionSyntax left2 = assignment2.Left;
                        ExpressionSyntax right2 = assignment2.Right;

                        if (left2?.IsMissing == false
                            && right2?.IsMissing == false
                            && IsEquivalent(left1, left2))
                        {
                            if (options.UseCoalesceExpression)
                            {
                                SyntaxKind conditionKind = condition.Kind();

                                if (conditionKind == SyntaxKind.EqualsExpression)
                                {
                                    var binaryExpression = (BinaryExpressionSyntax)condition;

                                    if (IsNullLiteral(binaryExpression.Right)
                                        && IsEquivalent(binaryExpression.Left, right2))
                                    {
                                        return new IfElseToAssignmentWithCoalesceExpression(ifStatement, condition, left1, right2, right1).ToImmutableArray();
                                    }
                                }
                                else if (conditionKind == SyntaxKind.NotEqualsExpression)
                                {
                                    var binaryExpression = (BinaryExpressionSyntax)condition;

                                    if (IsNullLiteral(binaryExpression.Right)
                                        && IsEquivalent(binaryExpression.Left, right1))
                                    {
                                        return new IfElseToAssignmentWithCoalesceExpression(ifStatement, condition, left1, right1, right2).ToImmutableArray();
                                    }
                                }
                            }

                            if (options.UseConditionalExpression)
                                return new IfElseToAssignmentWithConditionalExpression(ifStatement, left1, right1, right2).ToImmutableArray();
                        }
                    }
                }
            }

            return ImmutableArray<IfRefactoring>.Empty;
        }

        public static ImmutableArray<IfRefactoring> Analyze(
            SelectedStatementCollection selectedStatements,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return Analyze(selectedStatements, DefaultOptions, semanticModel, cancellationToken);
        }

        public static ImmutableArray<IfRefactoring> Analyze(
            SelectedStatementCollection selectedStatements,
            IfAnalysisOptions options,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (selectedStatements.Count == 2)
            {
                StatementSyntax[] statements = selectedStatements.ToArray();

                if (statements[0].IsKind(SyntaxKind.IfStatement)
                    && statements[1].IsKind(SyntaxKind.ReturnStatement))
                {
                    var ifStatement = (IfStatementSyntax)statements[0];

                    if (!IfElseChain.IsPartOfChain(ifStatement))
                        return Analyze(ifStatement, (ReturnStatementSyntax)statements[1], options, semanticModel, cancellationToken);
                }
            }

            return ImmutableArray<IfRefactoring>.Empty;
        }

        private static ImmutableArray<IfRefactoring> Analyze(
            IfStatementSyntax ifStatement,
            ReturnStatementSyntax returnStatement,
            IfAnalysisOptions options,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = ifStatement.Condition;

            if (condition?.IsMissing == false)
            {
                StatementSyntax statement = GetSingleStatementOrDefault(ifStatement);

                if (statement?.IsKind(SyntaxKind.ReturnStatement) == true
                    && options.CheckSpanDirectives(ifStatement, TextSpan.FromBounds(ifStatement.SpanStart, returnStatement.Span.End)))
                {
                    return Analyze(
                        ifStatement,
                        condition,
                        ((ReturnStatementSyntax)statement).Expression,
                        returnStatement.Expression,
                        semanticModel,
                        cancellationToken,
                        options,
                        isYield: false);
                }
            }

            return ImmutableArray<IfRefactoring>.Empty;
        }

        private ImmutableArray<IfRefactoring> ToImmutableArray()
        {
            return ImmutableArray.Create(this);
        }

        private static ImmutableArray<IfRefactoring> ToImmutableArray(IfRefactoring refactoring1, IfRefactoring refactoring2)
        {
            if (refactoring1 != null)
            {
                if (refactoring2 != null)
                {
                    return ImmutableArray.Create(refactoring1, refactoring2);
                }
                else
                {
                    return refactoring1.ToImmutableArray();
                }
            }
            else if (refactoring2 != null)
            {
                return refactoring2.ToImmutableArray();
            }

            return ImmutableArray<IfRefactoring>.Empty;
        }
    }
}
