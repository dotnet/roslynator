// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCompoundAssignmentRefactoring
    {
        public static bool CanRefactor(AssignmentExpressionSyntax assignment)
        {
            if (assignment == null)
                throw new ArgumentNullException(nameof(assignment));

            if (assignment.IsKind(SyntaxKind.SimpleAssignmentExpression))
            {
                ExpressionSyntax left = assignment.Left;
                ExpressionSyntax right = assignment.Right;

                if (left?.IsMissing == false
                    && right?.IsMissing == false
                    && !assignment.IsParentKind(SyntaxKind.ObjectInitializerExpression)
                    && right.Kind().SupportsCompoundAssignment())
                {
                    var binaryExpression = (BinaryExpressionSyntax)right;
                    ExpressionSyntax binaryLeft = binaryExpression.Left;
                    ExpressionSyntax binaryRight = binaryExpression.Right;

                    return binaryLeft?.IsMissing == false
                        && binaryRight?.IsMissing == false
                        && SyntaxComparer.AreEquivalent(left, binaryLeft)
                        && (assignment
                            .DescendantTrivia(assignment.Span)
                            .All(f => f.IsWhitespaceOrEndOfLineTrivia()));
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            AssignmentExpressionSyntax assignmentExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (assignmentExpression == null)
                throw new ArgumentNullException(nameof(assignmentExpression));

            var binaryExpression = (BinaryExpressionSyntax)assignmentExpression.Right;

            AssignmentExpressionSyntax newNode = AssignmentExpression(
                GetCompoundAssignmentKind(binaryExpression),
                assignmentExpression.Left,
                binaryExpression.Right);

            newNode = newNode
                .WithTriviaFrom(assignmentExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(assignmentExpression, newNode, cancellationToken);
        }

        public static string GetCompoundOperatorText(BinaryExpressionSyntax binaryExpression)
        {
            return Token(GetCompoundOperatorKind(binaryExpression)).ToString();
        }

        private static SyntaxKind GetCompoundOperatorKind(BinaryExpressionSyntax binaryExpression)
        {
            switch (binaryExpression.Kind())
            {
                case SyntaxKind.AddExpression:
                    return SyntaxKind.PlusEqualsToken;
                case SyntaxKind.SubtractExpression:
                    return SyntaxKind.MinusEqualsToken;
                case SyntaxKind.MultiplyExpression:
                    return SyntaxKind.AsteriskEqualsToken;
                case SyntaxKind.DivideExpression:
                    return SyntaxKind.SlashEqualsToken;
                case SyntaxKind.ModuloExpression:
                    return SyntaxKind.PercentEqualsToken;
                case SyntaxKind.LeftShiftExpression:
                    return SyntaxKind.LessThanLessThanEqualsToken;
                case SyntaxKind.RightShiftExpression:
                    return SyntaxKind.GreaterThanGreaterThanEqualsToken;
                case SyntaxKind.BitwiseOrExpression:
                    return SyntaxKind.BarEqualsToken;
                case SyntaxKind.BitwiseAndExpression:
                    return SyntaxKind.AmpersandEqualsToken;
                case SyntaxKind.ExclusiveOrExpression:
                    return SyntaxKind.CaretEqualsToken;
                default:
                    {
                        Debug.Fail(binaryExpression.Kind().ToString());
                        return SyntaxKind.None;
                    }
            }
        }

        private static SyntaxKind GetCompoundAssignmentKind(BinaryExpressionSyntax binaryExpression)
        {
            switch (binaryExpression.Kind())
            {
                case SyntaxKind.AddExpression:
                    return SyntaxKind.AddAssignmentExpression;
                case SyntaxKind.SubtractExpression:
                    return SyntaxKind.SubtractAssignmentExpression;
                case SyntaxKind.MultiplyExpression:
                    return SyntaxKind.MultiplyAssignmentExpression;
                case SyntaxKind.DivideExpression:
                    return SyntaxKind.DivideAssignmentExpression;
                case SyntaxKind.ModuloExpression:
                    return SyntaxKind.ModuloAssignmentExpression;
                case SyntaxKind.LeftShiftExpression:
                    return SyntaxKind.LeftShiftAssignmentExpression;
                case SyntaxKind.RightShiftExpression:
                    return SyntaxKind.RightShiftAssignmentExpression;
                case SyntaxKind.BitwiseOrExpression:
                    return SyntaxKind.OrAssignmentExpression;
                case SyntaxKind.BitwiseAndExpression:
                    return SyntaxKind.AndAssignmentExpression;
                case SyntaxKind.ExclusiveOrExpression:
                    return SyntaxKind.ExclusiveOrAssignmentExpression;
                default:
                    {
                        Debug.Fail(binaryExpression.Kind().ToString());
                        return SyntaxKind.None;
                    }
            }
        }
    }
}
