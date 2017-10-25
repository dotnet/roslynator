// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
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
            if (ifStatement.IsTopmostIf())
            {
                ExpressionSyntax condition = ifStatement.Condition;

                if (condition != null)
                {
                    ElseClauseSyntax elseClause = ifStatement.Else;

                    if (elseClause != null)
                    {
                        if (options.CheckSpanDirectives(ifStatement))
                        {
                            StatementSyntax statement1 = ifStatement.GetSingleStatementOrDefault();

                            if (statement1 != null)
                            {
                                SyntaxKind kind1 = statement1.Kind();

                                if (kind1 == SyntaxKind.ExpressionStatement
                                    || kind1 == SyntaxKind.ReturnStatement
                                    || kind1 == SyntaxKind.YieldReturnStatement)
                                {
                                    StatementSyntax statement2 = elseClause.GetSingleStatementOrDefault();

                                    if (statement2?.IsKind(kind1) == true)
                                    {
                                        switch (kind1)
                                        {
                                            case SyntaxKind.ExpressionStatement:
                                                {
                                                    return Analyze(ifStatement, condition, (ExpressionStatementSyntax)statement1, (ExpressionStatementSyntax)statement2, semanticModel, cancellationToken, options);
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
                        StatementSyntax nextStatement = ifStatement.NextStatementOrDefault();

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
                    NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(condition, semanticModel: semanticModel, cancellationToken: cancellationToken);
                    if (nullCheck.Success)
                    {
                        IfRefactoring refactoring = CreateIfToReturnWithCoalesceExpression(
                            ifStatement,
                            (nullCheck.IsCheckingNull) ? expression2 : expression1,
                            (nullCheck.IsCheckingNull) ? expression1 : expression2,
                            nullCheck,
                            isYield,
                            semanticModel,
                            cancellationToken);

                        if (refactoring != null)
                            return refactoring.ToImmutableArray();
                    }
                }

                IfToReturnWithBooleanExpression ifToReturnWithBooleanExpression = null;

                if (options.UseBooleanExpression
                    && (expression1.Kind().IsBooleanLiteralExpression() || expression2.Kind().IsBooleanLiteralExpression())
                    && semanticModel.GetTypeSymbol(expression1, cancellationToken)?.IsBoolean() == true
                    && semanticModel.GetTypeSymbol(expression2, cancellationToken)?.IsBoolean() == true)
                {
                    ifToReturnWithBooleanExpression = IfToReturnWithBooleanExpression.Create(ifStatement, expression1, expression2, isYield);
                }

                IfToReturnWithConditionalExpression ifToReturnWithConditionalExpression = null;

                if (options.UseConditionalExpression
                    && (!expression1.Kind().IsBooleanLiteralExpression() || !expression2.Kind().IsBooleanLiteralExpression()))
                {
                    ifToReturnWithConditionalExpression = IfToReturnWithConditionalExpression.Create(ifStatement, expression1, expression2, isYield);
                }

                return ToImmutableArray(ifToReturnWithBooleanExpression, ifToReturnWithConditionalExpression);
            }

            return ImmutableArray<IfRefactoring>.Empty;
        }

        private static IfRefactoring CreateIfToReturnWithCoalesceExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2,
            NullCheckExpressionInfo nullCheck,
            bool isYield,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (nullCheck.Kind == NullCheckKind.EqualsToNull
                || nullCheck.Kind == NullCheckKind.NotEqualsToNull)
            {
                if (SyntaxComparer.AreEquivalent(nullCheck.Expression, expression1, requireNotNull: true))
                {
                    return IfToReturnWithCoalesceExpression.Create(ifStatement, expression1, expression2, isYield);
                }
            }

            if (expression1.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                && SyntaxUtility.IsPropertyOfNullableOfT(expression1, "Value", semanticModel, cancellationToken))
            {
                expression1 = ((MemberAccessExpressionSyntax)expression1).Expression;

                if (SyntaxComparer.AreEquivalent(nullCheck.Expression, expression1, requireNotNull: true))
                {
                    return IfToReturnWithCoalesceExpression.Create(ifStatement, expression1, expression2, isYield);
                }
            }

            return null;
        }

        private static ImmutableArray<IfRefactoring> Analyze(
            IfStatementSyntax ifStatement,
            ExpressionSyntax condition,
            ExpressionStatementSyntax expressionStatement1,
            ExpressionStatementSyntax expressionStatement2,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
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
                            && SyntaxComparer.AreEquivalent(left1, left2))
                        {
                            if (options.UseCoalesceExpression)
                            {
                                NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(condition, semanticModel: semanticModel, cancellationToken: cancellationToken);
                                if (nullCheck.Success)
                                {
                                    IfRefactoring refactoring = CreateIfToAssignmentWithWithCoalesceExpression(
                                        ifStatement,
                                        left1,
                                        (nullCheck.IsCheckingNull) ? right2 : right1,
                                        (nullCheck.IsCheckingNull) ? right1 : right2,
                                        nullCheck,
                                        semanticModel,
                                        cancellationToken);

                                    if (refactoring != null)
                                        return refactoring.ToImmutableArray();
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

        private static IfRefactoring CreateIfToAssignmentWithWithCoalesceExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2,
            NullCheckExpressionInfo nullCheck,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (nullCheck.Kind == NullCheckKind.EqualsToNull
                || nullCheck.Kind == NullCheckKind.NotEqualsToNull)
            {
                if (SyntaxComparer.AreEquivalent(nullCheck.Expression, expression1, requireNotNull: true))
                    return new IfElseToAssignmentWithCoalesceExpression(ifStatement, left, expression1, expression2);
            }

            if (expression1.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                && SyntaxUtility.IsPropertyOfNullableOfT(expression1, "Value", semanticModel, cancellationToken))
            {
                expression1 = ((MemberAccessExpressionSyntax)expression1).Expression;

                if (SyntaxComparer.AreEquivalent(nullCheck.Expression, expression1, requireNotNull: true))
                    return new IfElseToAssignmentWithCoalesceExpression(ifStatement, left, expression1, expression2);
            }

            return null;
        }

        public static ImmutableArray<IfRefactoring> Analyze(
            StatementsSelection selectedStatements,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return Analyze(selectedStatements, DefaultOptions, semanticModel, cancellationToken);
        }

        public static ImmutableArray<IfRefactoring> Analyze(
            StatementsSelection selectedStatements,
            IfAnalysisOptions options,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (selectedStatements.Count == 2)
            {
                StatementSyntax[] statements = selectedStatements.ToArray();

                StatementSyntax statement1 = statements[0];
                StatementSyntax statement2 = statements[1];

                if (!statement1.ContainsDiagnostics
                    && !statement2.ContainsDiagnostics)
                {
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
                                return Analyze((LocalDeclarationStatementSyntax)statement1, (IfStatementSyntax)statement2, options);
                        }
                        else if (kind1 == SyntaxKind.ExpressionStatement
                            && kind2 == SyntaxKind.IfStatement)
                        {
                            return Analyze((ExpressionStatementSyntax)statement1, (IfStatementSyntax)statement2, options);
                        }
                    }
                }
            }

            return ImmutableArray<IfRefactoring>.Empty;
        }

        private static ImmutableArray<IfRefactoring> Analyze(
            LocalDeclarationStatementSyntax localDeclarationStatement,
            IfStatementSyntax ifStatement,
            IfAnalysisOptions options)
        {
            VariableDeclaratorSyntax declarator = localDeclarationStatement
                .Declaration?
                .Variables
                .SingleOrDefault(throwException: false);

            if (declarator != null)
            {
                ElseClauseSyntax elseClause = ifStatement.Else;

                if (elseClause?.Statement?.IsKind(SyntaxKind.IfStatement) == false)
                {
                    SimpleAssignmentStatementInfo assignmentInfo1 = SyntaxInfo.SimpleAssignmentStatementInfo(ifStatement.GetSingleStatementOrDefault());
                    if (assignmentInfo1.Success)
                    {
                        SimpleAssignmentStatementInfo assignmentInfo2 = SyntaxInfo.SimpleAssignmentStatementInfo(elseClause.GetSingleStatementOrDefault());
                        if (assignmentInfo2.Success
                            && assignmentInfo1.Left.IsKind(SyntaxKind.IdentifierName)
                            && assignmentInfo2.Left.IsKind(SyntaxKind.IdentifierName))
                        {
                            string identifier1 = ((IdentifierNameSyntax)assignmentInfo1.Left).Identifier.ValueText;
                            string identifier2 = ((IdentifierNameSyntax)assignmentInfo2.Left).Identifier.ValueText;

                            if (string.Equals(identifier1, identifier2, StringComparison.Ordinal)
                                && string.Equals(identifier1, declarator.Identifier.ValueText, StringComparison.Ordinal)
                                && options.CheckSpanDirectives(ifStatement.Parent, TextSpan.FromBounds(localDeclarationStatement.SpanStart, ifStatement.Span.End)))
                            {
                                return new LocalDeclarationAndIfElseAssignmentWithConditionalExpression(localDeclarationStatement, ifStatement, assignmentInfo1.Right, assignmentInfo2.Right).ToImmutableArray();
                            }
                        }
                    }
                }
            }

            return ImmutableArray<IfRefactoring>.Empty;
        }

        private static ImmutableArray<IfRefactoring> Analyze(
            ExpressionStatementSyntax expressionStatement,
            IfStatementSyntax ifStatement,
            IfAnalysisOptions options)
        {
            SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo(expressionStatement);
            if (assignmentInfo.Success)
            {
                ElseClauseSyntax elseClause = ifStatement.Else;

                if (elseClause?.Statement?.IsKind(SyntaxKind.IfStatement) == false)
                {
                    SimpleAssignmentStatementInfo assignmentInfo1 = SyntaxInfo.SimpleAssignmentStatementInfo(ifStatement.GetSingleStatementOrDefault());
                    if (assignmentInfo1.Success)
                    {
                        SimpleAssignmentStatementInfo assignmentInfo2 = SyntaxInfo.SimpleAssignmentStatementInfo(elseClause.GetSingleStatementOrDefault());
                        if (assignmentInfo2.Success
                            && SyntaxComparer.AreEquivalent(assignmentInfo1.Left, assignmentInfo2.Left, assignmentInfo.Left)
                            && options.CheckSpanDirectives(ifStatement.Parent, TextSpan.FromBounds(expressionStatement.SpanStart, ifStatement.Span.End)))
                        {
                            return new AssignmentAndIfElseToAssignmentWithConditionalExpression(expressionStatement, assignmentInfo.Right, ifStatement, assignmentInfo1.Right, assignmentInfo2.Right).ToImmutableArray();
                        }
                    }
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
                StatementSyntax statement = ifStatement.GetSingleStatementOrDefault();

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
