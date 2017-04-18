// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UsePostfixUnaryOperatorInsteadOfAssignmentRefactoring
    {
        private static DiagnosticDescriptor FadeOutDescriptor { get; } = DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignmentFadeOut;

        public static void Analyze(SyntaxNodeAnalysisContext context, AssignmentExpressionSyntax assignment)
        {
            switch (assignment.Kind())
            {
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                    {
                        ExpressionSyntax left = assignment.Left;
                        ExpressionSyntax right = assignment.Right;

                        if (left?.IsMissing == false
                            && right?.IsNumericLiteralExpression(1) == true)
                        {
                            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(left, context.CancellationToken);

                            if (typeSymbol?.SupportsPrefixOrPostfixUnaryOperator() == true
                                && !assignment.SpanContainsDirectives())
                            {
                                ReportDiagnostic(context, assignment);

                                SyntaxToken operatorToken = assignment.OperatorToken;

                                if (operatorToken.Span.Length == 2)
                                    context.ReportDiagnostic(FadeOutDescriptor, Location.Create(assignment.SyntaxTree, new TextSpan(operatorToken.SpanStart, 1)));

                                context.ReportNode(FadeOutDescriptor, assignment.Right);
                            }
                        }

                        break;
                    }
                case SyntaxKind.SimpleAssignmentExpression:
                    {
                        ExpressionSyntax left = assignment.Left;
                        ExpressionSyntax right = assignment.Right;

                        if (left?.IsMissing == false
                            && right?.IsMissing == false
                            && right.IsKind(SyntaxKind.AddExpression, SyntaxKind.SubtractExpression))
                        {
                            var binaryExpression = (BinaryExpressionSyntax)right;
                            ExpressionSyntax binaryLeft = binaryExpression.Left;
                            ExpressionSyntax binaryRight = binaryExpression.Right;

                            if (binaryLeft?.IsMissing == false
                                && binaryRight?.IsNumericLiteralExpression(1) == true)
                            {
                                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(left, context.CancellationToken);

                                if (typeSymbol?.SupportsPrefixOrPostfixUnaryOperator() == true
                                    && left.IsEquivalentTo(binaryLeft, topLevel: false)
                                    && !assignment.SpanContainsDirectives())
                                {
                                    ReportDiagnostic(context, assignment);

                                    context.ReportToken(FadeOutDescriptor, assignment.OperatorToken);
                                    context.ReportNode(FadeOutDescriptor, binaryLeft);
                                    context.ReportNode(FadeOutDescriptor, binaryRight);
                                }
                            }
                        }

                        break;
                    }
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, AssignmentExpressionSyntax assignment)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignment,
                assignment,
                GetOperatorText(assignment));
        }

        public static Task<Document> RefactorAsync(
            Document document,
            AssignmentExpressionSyntax assignment,
            SyntaxKind kind,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            PostfixUnaryExpressionSyntax postfixUnary = PostfixUnaryExpression(kind, assignment.Left)
                .WithTrailingTrivia(GetTrailingTrivia(assignment))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(assignment, postfixUnary, cancellationToken);
        }

        private static List<SyntaxTrivia> GetTrailingTrivia(AssignmentExpressionSyntax assignment)
        {
            var trivia = new List<SyntaxTrivia>();

            ExpressionSyntax right = assignment.Right;

            switch (assignment.Kind())
            {
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                    {
                        trivia.AddRange(assignment.OperatorToken.GetLeadingAndTrailingTrivia());

                        if (right?.IsMissing == false)
                            trivia.AddRange(right.GetLeadingAndTrailingTrivia());

                        return trivia;
                    }
            }

            switch (right?.Kind())
            {
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                    {
                        trivia.AddRange(assignment.OperatorToken.GetLeadingAndTrailingTrivia());

                        if (right?.IsMissing == false)
                        {
                            var binaryExpression = (BinaryExpressionSyntax)right;

                            trivia.AddRange(binaryExpression.DescendantTrivia());
                        }

                        return trivia;
                    }
            }

            Debug.Assert(false, assignment.Kind().ToString());

            return trivia;
        }

        public static SyntaxKind GetPostfixUnaryOperatorKind(AssignmentExpressionSyntax assignment)
        {
            if (assignment == null)
                throw new ArgumentNullException(nameof(assignment));

            switch (assignment.Kind())
            {
                case SyntaxKind.AddAssignmentExpression:
                    return SyntaxKind.PostIncrementExpression;
                case SyntaxKind.SubtractAssignmentExpression:
                    return SyntaxKind.PostDecrementExpression;
            }

            switch (assignment.Right?.Kind())
            {
                case SyntaxKind.AddExpression:
                    return SyntaxKind.PostIncrementExpression;
                case SyntaxKind.SubtractExpression:
                    return SyntaxKind.PostDecrementExpression;
            }

            Debug.Assert(false, assignment.Kind().ToString());

            return SyntaxKind.None;
        }

        private static string GetOperatorText(AssignmentExpressionSyntax assignment)
        {
            return GetOperatorText(GetPostfixUnaryOperatorKind(assignment));
        }

        public static string GetOperatorText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PostIncrementExpression:
                    return "++";
                case SyntaxKind.PostDecrementExpression:
                    return "--";
                default:
                    {
                        Debug.Assert(false, kind.ToString());
                        return "";
                    }
            }
        }
    }
}
