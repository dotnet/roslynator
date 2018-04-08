// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.CSharpFacts;

namespace Roslynator.CSharp.Analysis.If
{
    internal abstract class IfAnalysis
    {
        public static IfAnalysisOptions DefaultOptions { get; } = new IfAnalysisOptions();

        private static ImmutableArray<IfAnalysis> Empty
        {
            get { return ImmutableArray<IfAnalysis>.Empty; }
        }

        protected IfAnalysis(IfStatementSyntax ifStatement, SemanticModel semanticModel)
        {
            IfStatement = ifStatement;
            SemanticModel = semanticModel;
        }

        public abstract IfAnalysisKind Kind { get; }

        public abstract string Title { get; }

        public IfStatementSyntax IfStatement { get; }

        public SemanticModel SemanticModel { get; }

        public static ImmutableArray<IfAnalysis> Analyze(
            IfStatementSyntax ifStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Analyze(ifStatement, DefaultOptions, semanticModel, cancellationToken);
        }

        public static ImmutableArray<IfAnalysis> Analyze(
            IfStatementSyntax ifStatement,
            IfAnalysisOptions options,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ifStatement.IsTopmostIf())
                return Empty;

            ExpressionSyntax condition = ifStatement.Condition?.WalkDownParentheses();

            if (condition == null)
                return Empty;

            ElseClauseSyntax elseClause = ifStatement.Else;

            if (elseClause != null)
            {
                if (!options.CheckSpanDirectives(ifStatement))
                    return Empty;

                StatementSyntax statement1 = ifStatement.SingleNonBlockStatementOrDefault();

                if (statement1 == null)
                    return Empty;

                SyntaxKind kind1 = statement1.Kind();

                if (kind1.Is(
                    SyntaxKind.ExpressionStatement,
                    SyntaxKind.ReturnStatement,
                    SyntaxKind.YieldReturnStatement))
                {
                    StatementSyntax statement2 = elseClause.SingleNonBlockStatementOrDefault();

                    if (statement2?.Kind() == kind1)
                    {
                        switch (kind1)
                        {
                            case SyntaxKind.ExpressionStatement:
                                {
                                    return Analyze(
                                        ifStatement,
                                        condition,
                                        (ExpressionStatementSyntax)statement1,
                                        (ExpressionStatementSyntax)statement2,
                                        options,
                                        semanticModel,
                                        cancellationToken);
                                }
                            case SyntaxKind.ReturnStatement:
                                {
                                    return Analyze(
                                        ifStatement,
                                        condition,
                                        ((ReturnStatementSyntax)statement1).Expression?.WalkDownParentheses(),
                                        ((ReturnStatementSyntax)statement2).Expression?.WalkDownParentheses(),
                                        options,
                                        isYield: false,
                                        semanticModel: semanticModel,
                                        cancellationToken: cancellationToken);
                                }
                            case SyntaxKind.YieldReturnStatement:
                                {
                                    return Analyze(
                                        ifStatement,
                                        condition,
                                        ((YieldStatementSyntax)statement1).Expression?.WalkDownParentheses(),
                                        ((YieldStatementSyntax)statement2).Expression?.WalkDownParentheses(),
                                        options,
                                        isYield: true,
                                        semanticModel: semanticModel,
                                        cancellationToken: cancellationToken);
                                }
                        }
                    }
                }
            }
            else if (ifStatement.NextStatement() is ReturnStatementSyntax returnStatement)
            {
                return Analyze(ifStatement, returnStatement, options, semanticModel, cancellationToken);
            }

            return Empty;
        }

        private static ImmutableArray<IfAnalysis> Analyze(
            IfStatementSyntax ifStatement,
            ExpressionSyntax condition,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2,
            IfAnalysisOptions options,
            bool isYield,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (expression1?.IsMissing != false)
                return Empty;

            if (expression2?.IsMissing != false)
                return Empty;

            if (options.UseCoalesceExpression
                || options.UseExpression)
            {
                SyntaxKind kind1 = expression1.Kind();
                SyntaxKind kind2 = expression2.Kind();

                if (IsBooleanLiteralExpression(kind1)
                    && IsBooleanLiteralExpression(kind2)
                    && kind1 != kind2)
                {
                    if (options.UseExpression)
                    {
                        if (ifStatement.IsSimpleIf()
                            && (ifStatement.PreviousStatement() is IfStatementSyntax previousIf)
                            && previousIf.IsSimpleIf()
                            && (previousIf.SingleNonBlockStatementOrDefault() is ReturnStatementSyntax returnStatement)
                            && returnStatement.Expression?.WalkDownParentheses().Kind() == kind1)
                        {
                            return Empty;
                        }

                        return new IfToReturnWithExpressionAnalysis(ifStatement, condition, isYield, negate: kind1 == SyntaxKind.FalseLiteralExpression, semanticModel: semanticModel).ToImmutableArray();
                    }

                    return Empty;
                }

                NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(condition, semanticModel: semanticModel, cancellationToken: cancellationToken);

                if (nullCheck.Success)
                {
                    IfAnalysis refactoring = CreateIfToReturnStatement(
                        ifStatement,
                        (nullCheck.IsCheckingNull) ? expression2 : expression1,
                        (nullCheck.IsCheckingNull) ? expression1 : expression2,
                        nullCheck,
                        options,
                        isYield,
                        semanticModel,
                        cancellationToken);

                    if (refactoring != null)
                        return refactoring.ToImmutableArray();
                }
            }

            IfToReturnWithBooleanExpressionAnalysis ifToReturnWithBooleanExpression = null;

            if (options.UseBooleanExpression
                && (IsBooleanLiteralExpression(expression1.Kind()) || IsBooleanLiteralExpression(expression2.Kind()))
                && semanticModel.GetTypeSymbol(expression1, cancellationToken)?.SpecialType == SpecialType.System_Boolean
                && semanticModel.GetTypeSymbol(expression2, cancellationToken)?.SpecialType == SpecialType.System_Boolean)
            {
                ifToReturnWithBooleanExpression = IfToReturnWithBooleanExpressionAnalysis.Create(ifStatement, expression1, expression2, semanticModel, isYield);
            }

            IfToReturnWithConditionalExpressionAnalysis ifToReturnWithConditionalExpression = null;

            if (options.UseConditionalExpression
                && (!IsBooleanLiteralExpression(expression1.Kind()) || !IsBooleanLiteralExpression(expression2.Kind())))
            {
                ifToReturnWithConditionalExpression = IfToReturnWithConditionalExpressionAnalysis.Create(ifStatement, expression1, expression2, semanticModel, isYield);
            }

            return ToImmutableArray(ifToReturnWithBooleanExpression, ifToReturnWithConditionalExpression);
        }

        private static IfAnalysis CreateIfToReturnStatement(
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2,
            NullCheckExpressionInfo nullCheck,
            IfAnalysisOptions options,
            bool isYield,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if ((nullCheck.Style & NullCheckStyles.ComparisonToNull) != 0
                && AreEquivalent(nullCheck.Expression, expression1))
            {
                return CreateIfToReturnStatement(isNullable: false);
            }

            expression1 = GetNullableOfTValueProperty(expression1, semanticModel, cancellationToken);

            if (AreEquivalent(nullCheck.Expression, expression1))
                return CreateIfToReturnStatement(isNullable: true);

            return null;

            IfAnalysis CreateIfToReturnStatement(bool isNullable)
            {
                if (!isNullable
                    && expression2.Kind() == SyntaxKind.NullLiteralExpression)
                {
                    if (options.UseExpression)
                        return new IfToReturnWithExpressionAnalysis(ifStatement, expression1, isYield, false, semanticModel);
                }
                else if (options.UseCoalesceExpression)
                {
                    return new IfToReturnWithCoalesceExpressionAnalysis(ifStatement, expression1, expression2, semanticModel, isYield);
                }

                return null;
            }
        }

        private static ImmutableArray<IfAnalysis> Analyze(
            IfStatementSyntax ifStatement,
            ExpressionSyntax condition,
            ExpressionStatementSyntax expressionStatement1,
            ExpressionStatementSyntax expressionStatement2,
            IfAnalysisOptions options,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SimpleAssignmentStatementInfo assignment1 = SyntaxInfo.SimpleAssignmentStatementInfo(expressionStatement1);

            if (!assignment1.Success)
                return Empty;

            SimpleAssignmentStatementInfo assignment2 = SyntaxInfo.SimpleAssignmentStatementInfo(expressionStatement2);

            if (!assignment2.Success)
                return Empty;

            ExpressionSyntax left1 = assignment1.Left;
            ExpressionSyntax left2 = assignment2.Left;
            ExpressionSyntax right1 = assignment1.Right;
            ExpressionSyntax right2 = assignment2.Right;

            if (!AreEquivalent(left1, left2))
                return Empty;

            if (options.UseCoalesceExpression
                || options.UseExpression)
            {
                SyntaxKind kind1 = right1.Kind();
                SyntaxKind kind2 = right2.Kind();

                if (IsBooleanLiteralExpression(kind1)
                    && IsBooleanLiteralExpression(kind2)
                    && kind1 != kind2)
                {
                    if (options.UseExpression)
                        return new IfElseToAssignmentWithConditionAnalysis(ifStatement, left1, condition, semanticModel, negate: kind1 == SyntaxKind.FalseLiteralExpression).ToImmutableArray();

                    return Empty;
                }

                NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(condition, semanticModel: semanticModel, cancellationToken: cancellationToken);

                if (nullCheck.Success)
                {
                    IfAnalysis refactoring = CreateIfToAssignment(
                        ifStatement,
                        left1,
                        (nullCheck.IsCheckingNull) ? right2 : right1,
                        (nullCheck.IsCheckingNull) ? right1 : right2,
                        nullCheck,
                        options,
                        semanticModel,
                        cancellationToken);

                    if (refactoring != null)
                        return refactoring.ToImmutableArray();
                }
            }

            if (options.UseConditionalExpression)
                return new IfElseToAssignmentWithConditionalExpressionAnalysis(ifStatement, left1, right1, right2, semanticModel).ToImmutableArray();

            return Empty;
        }

        private static IfAnalysis CreateIfToAssignment(
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2,
            NullCheckExpressionInfo nullCheck,
            IfAnalysisOptions options,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if ((nullCheck.Style & NullCheckStyles.ComparisonToNull) != 0
                && AreEquivalent(nullCheck.Expression, expression1))
            {
                return CreateIfToAssignment(isNullable: false);
            }

            expression1 = GetNullableOfTValueProperty(expression1, semanticModel, cancellationToken);

            if (AreEquivalent(nullCheck.Expression, expression1))
                return CreateIfToAssignment(isNullable: true);

            return null;

            IfAnalysis CreateIfToAssignment(bool isNullable)
            {
                if (!isNullable
                    && expression2.Kind() == SyntaxKind.NullLiteralExpression)
                {
                    if (options.UseExpression)
                        return new IfElseToAssignmentWithExpressionAnalysis(ifStatement, expression1.FirstAncestor<ExpressionStatementSyntax>(), semanticModel);
                }
                else if (options.UseCoalesceExpression)
                {
                    return new IfElseToAssignmentWithCoalesceExpressionAnalysis(ifStatement, left, expression1, expression2, semanticModel);
                }

                return null;
            }
        }

        public static ImmutableArray<IfAnalysis> Analyze(
            StatementListSelection selectedStatements,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return Analyze(selectedStatements, DefaultOptions, semanticModel, cancellationToken);
        }

        public static ImmutableArray<IfAnalysis> Analyze(
            StatementListSelection selectedStatements,
            IfAnalysisOptions options,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (selectedStatements.Count != 2)
                return Empty;

            StatementSyntax[] statements = selectedStatements.ToArray();

            StatementSyntax statement1 = statements[0];
            StatementSyntax statement2 = statements[1];

            if (statement1.ContainsDiagnostics)
                return Empty;

            if (statement2.ContainsDiagnostics)
                return Empty;

            SyntaxKind kind1 = statement1.Kind();
            SyntaxKind kind2 = statement2.Kind();

            if (kind1 == SyntaxKind.IfStatement)
            {
                if (kind2 == SyntaxKind.ReturnStatement)
                {
                    var ifStatement = (IfStatementSyntax)statement1;

                    if (ifStatement.IsSimpleIf())
                        return Analyze(ifStatement, (ReturnStatementSyntax)statement2, options, semanticModel, cancellationToken);
                }
            }
            else if (options.UseConditionalExpression)
            {
                if (kind1 == SyntaxKind.LocalDeclarationStatement)
                {
                    if (kind2 == SyntaxKind.IfStatement)
                        return Analyze((LocalDeclarationStatementSyntax)statement1, (IfStatementSyntax)statement2, options, semanticModel);
                }
                else if (kind1 == SyntaxKind.ExpressionStatement
                    && kind2 == SyntaxKind.IfStatement)
                {
                    return Analyze((ExpressionStatementSyntax)statement1, (IfStatementSyntax)statement2, options, semanticModel);
                }
            }

            return Empty;
        }

        private static ImmutableArray<IfAnalysis> Analyze(
            LocalDeclarationStatementSyntax localDeclarationStatement,
            IfStatementSyntax ifStatement,
            IfAnalysisOptions options,
            SemanticModel semanticModel)
        {
            VariableDeclaratorSyntax declarator = localDeclarationStatement
                .Declaration?
                .Variables
                .SingleOrDefault(shouldThrow: false);

            if (declarator == null)
                return Empty;

            ElseClauseSyntax elseClause = ifStatement.Else;

            if (elseClause?.Statement?.IsKind(SyntaxKind.IfStatement) != false)
                return Empty;

            SimpleAssignmentStatementInfo assignment1 = SyntaxInfo.SimpleAssignmentStatementInfo(ifStatement.SingleNonBlockStatementOrDefault());

            if (!assignment1.Success)
                return Empty;

            SimpleAssignmentStatementInfo assignment2 = SyntaxInfo.SimpleAssignmentStatementInfo(elseClause.SingleNonBlockStatementOrDefault());

            if (!assignment2.Success)
                return Empty;

            if (assignment1.Left.Kind() != SyntaxKind.IdentifierName)
                return Empty;

            if (assignment2.Left.Kind() != SyntaxKind.IdentifierName)
                return Empty;

            string identifier1 = ((IdentifierNameSyntax)assignment1.Left).Identifier.ValueText;
            string identifier2 = ((IdentifierNameSyntax)assignment2.Left).Identifier.ValueText;

            if (!string.Equals(identifier1, identifier2, StringComparison.Ordinal))
                return Empty;

            if (!string.Equals(identifier1, declarator.Identifier.ValueText, StringComparison.Ordinal))
                return Empty;

            if (!options.CheckSpanDirectives(ifStatement.Parent, TextSpan.FromBounds(localDeclarationStatement.SpanStart, ifStatement.Span.End)))
                return Empty;

            return new LocalDeclarationAndIfElseToAssignmentWithConditionalExpressionAnalysis(localDeclarationStatement, ifStatement, assignment1.Right, assignment2.Right, semanticModel).ToImmutableArray();
        }

        private static ImmutableArray<IfAnalysis> Analyze(
            ExpressionStatementSyntax expressionStatement,
            IfStatementSyntax ifStatement,
            IfAnalysisOptions options,
            SemanticModel semanticModel)
        {
            SimpleAssignmentStatementInfo assignment = SyntaxInfo.SimpleAssignmentStatementInfo(expressionStatement);

            if (!assignment.Success)
                return Empty;

            ElseClauseSyntax elseClause = ifStatement.Else;

            if (elseClause?.Statement?.IsKind(SyntaxKind.IfStatement) != false)
                return Empty;

            SimpleAssignmentStatementInfo assignment1 = SyntaxInfo.SimpleAssignmentStatementInfo(ifStatement.SingleNonBlockStatementOrDefault());

            if (!assignment1.Success)
                return Empty;

            SimpleAssignmentStatementInfo assignment2 = SyntaxInfo.SimpleAssignmentStatementInfo(elseClause.SingleNonBlockStatementOrDefault());

            if (!assignment2.Success)
                return Empty;

            if (!AreEquivalent(assignment1.Left, assignment2.Left, assignment.Left))
                return Empty;

            if (!options.CheckSpanDirectives(ifStatement.Parent, TextSpan.FromBounds(expressionStatement.SpanStart, ifStatement.Span.End)))
                return Empty;

            return new AssignmentAndIfElseToAssignmentWithConditionalExpressionAnalysis(expressionStatement, assignment.Right, ifStatement, assignment1.Right, assignment2.Right, semanticModel).ToImmutableArray();
        }

        private static ImmutableArray<IfAnalysis> Analyze(
            IfStatementSyntax ifStatement,
            ReturnStatementSyntax returnStatement,
            IfAnalysisOptions options,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = ifStatement.Condition?.WalkDownParentheses();

            if (condition?.IsMissing != false)
                return Empty;

            StatementSyntax statement = ifStatement.SingleNonBlockStatementOrDefault();

            if (statement?.IsKind(SyntaxKind.ReturnStatement) != true)
                return Empty;

            if (!options.CheckSpanDirectives(ifStatement, TextSpan.FromBounds(ifStatement.SpanStart, returnStatement.Span.End)))
                return Empty;

            return Analyze(
                ifStatement,
                condition,
                ((ReturnStatementSyntax)statement).Expression?.WalkDownParentheses(),
                returnStatement.Expression?.WalkDownParentheses(),
                options,
                isYield: false,
                semanticModel: semanticModel,
                cancellationToken: cancellationToken);
        }

        private ImmutableArray<IfAnalysis> ToImmutableArray()
        {
            return ImmutableArray.Create(this);
        }

        private static ImmutableArray<IfAnalysis> ToImmutableArray(IfAnalysis refactoring1, IfAnalysis refactoring2)
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

            return Empty;
        }

        private static ExpressionSyntax GetNullableOfTValueProperty(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (expression.Kind() != SyntaxKind.SimpleMemberAccessExpression)
                return null;

            var memberAccessExpression = (MemberAccessExpressionSyntax)expression;

            if (!(memberAccessExpression.Name is IdentifierNameSyntax identifierName))
                return null;

            if (!string.Equals(identifierName.Identifier.ValueText, "Value", StringComparison.Ordinal))
                return null;

            if (!SyntaxUtility.IsPropertyOfNullableOfT(expression, "Value", semanticModel, cancellationToken))
                return null;

            return memberAccessExpression.Expression;
        }
    }
}
