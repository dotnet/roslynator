// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;
using Roslynator.Utilities;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseConditionalAccessRefactoring
    {
        public static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionType)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            if (ifStatement.IsSimpleIf()
                && !ifStatement.ContainsDiagnostics)
            {
                NotEqualsToNullExpression notEqualsToNull;
                if (NotEqualsToNullExpression.TryCreate(ifStatement.Condition, out notEqualsToNull))
                {
                    MemberInvocationStatement memberInvocation;
                    if (MemberInvocationStatement.TryCreate(ifStatement.GetSingleStatementOrDefault(), out memberInvocation)
                        && notEqualsToNull.Left.IsEquivalentTo(memberInvocation.Expression, topLevel: false)
                        && !ifStatement.IsInExpressionTree(expressionType, context.SemanticModel, context.CancellationToken)
                        && !ifStatement.SpanContainsDirectives())
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.UseConditionalAccess, ifStatement);
                    }
                }
            }
        }

        public static void AnalyzeLogicalAndExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionType)
        {
            var logicalAndExpression = (BinaryExpressionSyntax)context.Node;

            if (!logicalAndExpression.ContainsDiagnostics)
            {
                ExpressionSyntax expression = FindExpressionCheckedForNull(logicalAndExpression);

                if (expression != null
                    && context.SemanticModel
                        .GetTypeSymbol(expression, context.CancellationToken)?
                        .IsConstructedFrom(SpecialType.System_Nullable_T) == false)
                {
                    ExpressionSyntax right = logicalAndExpression.Right;

                    if (right != null
                        && ValidateRightExpression(right, context.SemanticModel, context.CancellationToken)
                        && !ContainsOutArgumentWithLocal(right, context.SemanticModel, context.CancellationToken))
                    {
                        ExpressionSyntax expression2 = FindExpressionThatCanBeConditionallyAccessed(expression, right);

                        if (expression2?.SpanContainsDirectives() == false
                            && !logicalAndExpression.IsInExpressionTree(expressionType, context.SemanticModel, context.CancellationToken))
                        {
                            context.ReportDiagnostic(DiagnosticDescriptors.UseConditionalAccess, logicalAndExpression);
                        }
                    }
                }
            }
        }

        public static void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionType)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            ConditionalExpressionInfo conditionalExpression;
            if (ConditionalExpressionInfo.TryCreate((ConditionalExpressionSyntax)context.Node, out conditionalExpression))
            {
                SemanticModel semanticModel = context.SemanticModel;
                CancellationToken cancellationToken = context.CancellationToken;

                NullCheckExpression nullCheck;
                if (NullCheckExpression.TryCreate(conditionalExpression.Condition, semanticModel, out nullCheck, cancellationToken))
                {
                    ExpressionSyntax whenNotNull = (nullCheck.IsCheckingNotNull)
                        ? conditionalExpression.WhenTrue
                        : conditionalExpression.WhenFalse;

                    if (whenNotNull.IsKind(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxKind.ElementAccessExpression,
                            SyntaxKind.ConditionalAccessExpression,
                            SyntaxKind.InvocationExpression)
                        && !ContainsOutArgumentWithLocal(whenNotNull, semanticModel, cancellationToken))
                    {
                        ExpressionSyntax expression = FindExpressionThatCanBeConditionallyAccessed(nullCheck.Expression, whenNotNull);

                        if (expression != null)
                        {
                            ExpressionSyntax whenNull = (nullCheck.IsCheckingNull)
                                ? conditionalExpression.WhenTrue
                                : conditionalExpression.WhenFalse;

                            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(whenNotNull, cancellationToken);

                            if (semanticModel.IsDefaultValue(typeSymbol, whenNull, cancellationToken)
                                && !conditionalExpression.Node.IsInExpressionTree(expressionType, semanticModel, cancellationToken))
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.UseConditionalAccessInsteadOfConditionalExpression,
                                    conditionalExpression.Node);
                            }
                        }
                    }
                }
            }
        }

        private static ExpressionSyntax FindExpressionCheckedForNull(BinaryExpressionSyntax logicalAndExpression)
        {
            ExpressionSyntax left = logicalAndExpression.Left;

            if (left?.IsKind(SyntaxKind.NotEqualsExpression) == true)
            {
                var notEquals = (BinaryExpressionSyntax)left;

                if (notEquals.Right?.IsKind(SyntaxKind.NullLiteralExpression) == true)
                    return notEquals.Left;
            }

            return null;
        }

        private static ExpressionSyntax FindExpressionThatCanBeConditionallyAccessed(ExpressionSyntax expressionToFind, ExpressionSyntax expression)
        {
            if (expression.IsKind(SyntaxKind.LogicalNotExpression))
                expression = ((PrefixUnaryExpressionSyntax)expression).Operand;

            SyntaxKind kind = expressionToFind.Kind();

            SyntaxToken firstToken = expression.GetFirstToken();

            int start = firstToken.SpanStart;

            SyntaxNode node = firstToken.Parent;

            while (node?.SpanStart == start)
            {
                if (kind == node.Kind()
                    && node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.ElementAccessExpression)
                    && expressionToFind.IsEquivalentTo(node, topLevel: false))
                {
                    return (ExpressionSyntax)node;
                }

                node = node.Parent;
            }

            return null;
        }

        private static bool ValidateRightExpression(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            SyntaxKind kind = expression.Kind();

            if (kind == SyntaxKind.EqualsExpression)
            {
                return ((BinaryExpressionSyntax)expression)
                    .Right?
                    .WalkDownParentheses()
                    .HasConstantNonNullValue(semanticModel, cancellationToken) == true;
            }
            else if (kind == SyntaxKind.NotEqualsExpression)
            {
                return ((BinaryExpressionSyntax)expression)
                    .Right?
                    .WalkDownParentheses()
                    .IsKind(SyntaxKind.NullLiteralExpression) == true;
            }
            else
            {
                return true;
            }
        }

        private static bool ContainsOutArgumentWithLocal(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            foreach (SyntaxNode node in expression.DescendantNodes())
            {
                if (node.IsKind(SyntaxKind.Argument))
                {
                    var argument = (ArgumentSyntax)node;

                    if (argument.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword))
                    {
                        ExpressionSyntax argumentExpression = argument.Expression;

                        if (argumentExpression?.IsMissing == false
                            && semanticModel.GetSymbol(argumentExpression, cancellationToken)?.IsLocal() == true)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool HasConstantNonNullValue(this ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            Optional<object> optional = semanticModel.GetConstantValue(expression, cancellationToken);

            return optional.HasValue
                && optional.Value != null;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax logicalAnd,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax newNode = CreateExpressionWithConditionalAccess(logicalAnd)
                .WithLeadingTrivia(logicalAnd.GetLeadingTrivia())
                .WithFormatterAnnotation()
                .Parenthesize();

            return document.ReplaceNodeAsync(logicalAnd, newNode, cancellationToken);
        }

        private static ExpressionSyntax CreateExpressionWithConditionalAccess(BinaryExpressionSyntax logicalAnd)
        {
            ExpressionSyntax expression = FindExpressionCheckedForNull(logicalAnd);

            ExpressionSyntax right = logicalAnd.Right;

            ExpressionSyntax expression2 = FindExpressionThatCanBeConditionallyAccessed(
                expression,
                right);

            SyntaxKind kind = right.Kind();

            if (kind == SyntaxKind.LogicalNotExpression)
            {
                var logicalNot = (PrefixUnaryExpressionSyntax)right;
                ExpressionSyntax operand = logicalNot.Operand;

                string s = operand.ToFullString();

                int length = expression2.Span.End - operand.FullSpan.Start;
                int trailingLength = operand.GetTrailingTrivia().Span.Length;

                var sb = new StringBuilder();
                sb.Append(s, 0, length);
                sb.Append("?");
                sb.Append(s, length, s.Length - length - trailingLength);
                sb.Append(" == false");
                sb.Append(s, s.Length - trailingLength, trailingLength);

                return SyntaxFactory.ParseExpression(sb.ToString());
            }
            else
            {
                string s = right.ToFullString();

                int length = expression2.Span.End - right.FullSpan.Start;
                int trailingLength = right.GetTrailingTrivia().Span.Length;

                var sb = new StringBuilder();
                sb.Append(s, 0, length);
                sb.Append("?");
                sb.Append(s, length, s.Length - length - trailingLength);

                switch (kind)
                {
                    case SyntaxKind.LogicalOrExpression:
                    case SyntaxKind.LogicalAndExpression:
                    case SyntaxKind.BitwiseOrExpression:
                    case SyntaxKind.BitwiseAndExpression:
                    case SyntaxKind.ExclusiveOrExpression:
                    case SyntaxKind.EqualsExpression:
                    case SyntaxKind.NotEqualsExpression:
                    case SyntaxKind.LessThanExpression:
                    case SyntaxKind.LessThanOrEqualExpression:
                    case SyntaxKind.GreaterThanExpression:
                    case SyntaxKind.GreaterThanOrEqualExpression:
                    case SyntaxKind.IsExpression:
                    case SyntaxKind.AsExpression:
                    case SyntaxKind.IsPatternExpression:
                        break;
                    default:
                        {
                            sb.Append(" == true");
                            break;
                        }
                }

                sb.Append(s, s.Length - trailingLength, trailingLength);

                return SyntaxFactory.ParseExpression(sb.ToString());
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            var statement = (ExpressionStatementSyntax)ifStatement.GetSingleStatementOrDefault();

            MemberInvocationStatement memberInvocation = MemberInvocationStatement.Create(statement);

            int insertIndex = memberInvocation.Expression.Span.End - statement.FullSpan.Start;
            StatementSyntax newStatement = SyntaxFactory.ParseStatement(statement.ToFullString().Insert(insertIndex, "?"));

            IEnumerable<SyntaxTrivia> leading = ifStatement.DescendantTrivia(TextSpan.FromBounds(ifStatement.SpanStart, statement.SpanStart));

            newStatement = (leading.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                ? newStatement.WithLeadingTrivia(ifStatement.GetLeadingTrivia())
                : newStatement.WithLeadingTrivia(ifStatement.GetLeadingTrivia().Concat(leading));

            IEnumerable<SyntaxTrivia> trailing = ifStatement.DescendantTrivia(TextSpan.FromBounds(statement.Span.End, ifStatement.Span.End));

            newStatement = (leading.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                ? newStatement.WithTrailingTrivia(ifStatement.GetTrailingTrivia())
                : newStatement.WithTrailingTrivia(trailing.Concat(ifStatement.GetTrailingTrivia()));

            return document.ReplaceNodeAsync(ifStatement, newStatement, cancellationToken);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpressionSyntax,
            CancellationToken cancellationToken)
        {
            ConditionalExpressionInfo conditionalExpression;
            if (ConditionalExpressionInfo.TryCreate(conditionalExpressionSyntax, out conditionalExpression))
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);

                NullCheckExpression nullCheck;
                if (NullCheckExpression.TryCreate(conditionalExpression.Condition, semanticModel, out nullCheck, cancellationToken))
                {
                    ExpressionSyntax whenNotNull = (nullCheck.IsCheckingNotNull)
                        ? conditionalExpression.WhenTrue
                        : conditionalExpression.WhenFalse;

                    ExpressionSyntax whenNull = (nullCheck.IsCheckingNull)
                        ? conditionalExpression.WhenTrue
                        : conditionalExpression.WhenFalse;

                    ExpressionSyntax expression = FindExpressionThatCanBeConditionallyAccessed(nullCheck.Expression, whenNotNull);

                    ExpressionSyntax newNode;

                    if (expression.Parent == whenNotNull
                        && whenNotNull.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                        && SemanticUtilities.IsPropertyOfNullableOfT(whenNotNull, "Value", semanticModel, cancellationToken))
                    {
                        newNode = expression;
                    }
                    else
                    {
                        newNode = SyntaxFactory.ParseExpression(whenNotNull.ToString().Insert(expression.Span.End - whenNotNull.SpanStart, "?"));
                    }

                    if (!semanticModel.GetTypeSymbol(whenNotNull, cancellationToken).IsReferenceType)
                        newNode = CSharpFactory.CoalesceExpression(newNode.Parenthesize(), whenNull.Parenthesize());

                    newNode = newNode
                        .WithTriviaFrom(conditionalExpressionSyntax)
                        .Parenthesize();

                    return await document.ReplaceNodeAsync(conditionalExpressionSyntax, newNode, cancellationToken).ConfigureAwait(false);
                }
            }

            Debug.Fail(conditionalExpressionSyntax.ToString());

            return document;
        }
    }
}
