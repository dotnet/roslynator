// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseConditionalAccessRefactoring
    {
        public static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
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
                        && !ifStatement.SpanContainsDirectives())
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.UseConditionalAccess, ifStatement);
                    }
                }
            }
        }

        public static void AnalyzeLogicalAndExpression(SyntaxNodeAnalysisContext context)
        {
            var logicalAndExpression = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax expression = FindExpressionCheckedForNull(logicalAndExpression);

            if (expression != null)
            {
                ExpressionSyntax right = logicalAndExpression.Right;

                if (right != null
                    && ValidateRightExpression(right, context.SemanticModel, context.CancellationToken)
                    && !ContainsOutArgumentWithLocal(right, context.SemanticModel, context.CancellationToken))
                {
                    SyntaxNode node = FindExpressionThatCanBeConditionallyAccessed(expression, right);

                    if (node?.SpanContainsDirectives() == false)
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.UseConditionalAccess, logicalAndExpression);
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

        private static SyntaxNode FindExpressionThatCanBeConditionallyAccessed(ExpressionSyntax expressionToFind, ExpressionSyntax expression)
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
                    return node;
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
                .Parenthesize(moveTrivia: true)
                .WithSimplifierAnnotation();

            return document.ReplaceNodeAsync(logicalAnd, newNode, cancellationToken);
        }

        private static ExpressionSyntax CreateExpressionWithConditionalAccess(BinaryExpressionSyntax logicalAnd)
        {
            ExpressionSyntax expression = FindExpressionCheckedForNull(logicalAnd);

            ExpressionSyntax right = logicalAnd.Right;

            SyntaxNode node = FindExpressionThatCanBeConditionallyAccessed(
                expression,
                right);

            SyntaxKind kind = right.Kind();

            if (kind == SyntaxKind.LogicalNotExpression)
            {
                var logicalNot = (PrefixUnaryExpressionSyntax)right;
                ExpressionSyntax operand = logicalNot.Operand;
                SyntaxToken operatorToken = logicalNot.OperatorToken;

                string s = operand.ToFullString();

                int length = node.Span.End - operand.FullSpan.Start;
                int trailingLength = operand.GetTrailingTrivia().Span.Length;

                var sb = new StringBuilder();
                sb.Append(s, 0, length);
                sb.Append("?");
                sb.Append(s, length, s.Length - length - trailingLength);
                sb.Append(" == false");
                sb.Append(s, s.Length - trailingLength, trailingLength);

                return ParseExpression(sb.ToString());
            }
            else
            {
                string s = right.ToFullString();

                int length = node.Span.End - right.FullSpan.Start;
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

                return ParseExpression(sb.ToString());
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
            StatementSyntax newStatement = ParseStatement(statement.ToFullString().Insert(insertIndex, "?"));

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
    }
}
