// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyBooleanExpressionRefactoring
    {
        internal static void AnalyzeLogicalAndExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var logicalAnd = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax left = logicalAnd.Left?.WalkDownParentheses();

            if (IsPropertyOfNullableOfT(left, "HasValue", context.SemanticModel, context.CancellationToken))
            {
                ExpressionSyntax right = logicalAnd.Right?.WalkDownParentheses();

                switch (right?.Kind())
                {
                    case SyntaxKind.LogicalNotExpression:
                        {
                            var logicalNot = (PrefixUnaryExpressionSyntax)right;

                            Analyze(context, logicalAnd, left, logicalNot.Operand?.WalkDownParentheses());
                            break;
                        }
                    case SyntaxKind.EqualsExpression:
                        {
                            var equalsExpression = (BinaryExpressionSyntax)right;

                            if (equalsExpression.Right?.WalkDownParentheses().IsKind(SyntaxKind.FalseLiteralExpression) == true)
                                Analyze(context, logicalAnd, left, equalsExpression.Left?.WalkDownParentheses());

                            break;
                        }
                    case SyntaxKind.SimpleMemberAccessExpression:
                        {
                            Analyze(context, logicalAnd, left, right);
                            break;
                        }
                }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax logicalAnd,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2)
        {
            if (IsPropertyOfNullableOfT(expression2, "Value", context.SemanticModel, context.CancellationToken)
                && SyntaxComparer.AreEquivalent(
                    ((MemberAccessExpressionSyntax)expression1).Expression,
                    ((MemberAccessExpressionSyntax)expression2).Expression,
                    requireNotNull: true))
            {
                context.ReportDiagnostic(DiagnosticDescriptors.SimplifyBooleanExpression, logicalAnd);
            }
        }

        private static bool IsPropertyOfNullableOfT(ExpressionSyntax expression, string name, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
            {
                var memberAccessExpression = (MemberAccessExpressionSyntax)expression;

                SimpleNameSyntax simpleName = memberAccessExpression.Name;

                if (simpleName?.IsKind(SyntaxKind.IdentifierName) == true)
                {
                    var identifierName = (IdentifierNameSyntax)simpleName;

                    return string.Equals(identifierName.Identifier.ValueText, name, StringComparison.Ordinal)
                        && SyntaxUtility.IsPropertyOfNullableOfT(expression, name, semanticModel, cancellationToken);
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax logicalAnd,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = logicalAnd.Left;
            ExpressionSyntax right = logicalAnd.Right;

            var memberAccessExpression = (MemberAccessExpressionSyntax)left.WalkDownParentheses();
            ExpressionSyntax expression = memberAccessExpression.Expression;

            SyntaxTriviaList trailingTrivia = logicalAnd
                .DescendantTrivia(TextSpan.FromBounds(expression.Span.End, left.Span.End))
                .ToSyntaxTriviaList()
                .EmptyIfWhitespace()
                .AddRange(left.GetTrailingTrivia());

            BinaryExpressionSyntax equalsExpression = EqualsExpression(
                expression
                    .WithLeadingTrivia(left.GetLeadingTrivia())
                    .WithTrailingTrivia(trailingTrivia),
                EqualsEqualsToken().WithTriviaFrom(logicalAnd.OperatorToken),
                (right.WalkDownParentheses().IsKind(SyntaxKind.LogicalNotExpression, SyntaxKind.EqualsExpression))
                    ? FalseLiteralExpression()
                    : TrueLiteralExpression().WithTriviaFrom(right));

            return document.ReplaceNodeAsync(logicalAnd, equalsExpression, cancellationToken);
        }
    }
}
