// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UsePostfixUnaryOperatorInsteadOfAssignmentRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            AssignmentExpressionSyntax assignment,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxKind kind = GetPostfixUnaryOperatorKind(assignment);

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
                        trivia.AddRange(assignment.OperatorToken.GetAllTrivia());

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
                        trivia.AddRange(assignment.OperatorToken.GetAllTrivia());

                        if (right?.IsMissing == false)
                        {
                            var binaryExpression = (BinaryExpressionSyntax)right;

                            trivia.AddRange(binaryExpression.DescendantTrivia());
                        }

                        return trivia;
                    }
            }

            Debug.Fail(assignment.Kind().ToString());

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

            Debug.Fail(assignment.Kind().ToString());

            return SyntaxKind.None;
        }

        public static string GetOperatorText(AssignmentExpressionSyntax assignment)
        {
            return GetOperatorText(GetPostfixUnaryOperatorKind(assignment));
        }

        private static string GetOperatorText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PostIncrementExpression:
                    return "++";
                case SyntaxKind.PostDecrementExpression:
                    return "--";
            }

            Debug.Fail(kind.ToString());

            return "";
        }
    }
}
