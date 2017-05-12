// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpandAssignmentExpressionRefactoring
    {
        public static bool CanRefactor(AssignmentExpressionSyntax assignmentExpression)
        {
            if (assignmentExpression == null)
                throw new ArgumentNullException(nameof(assignmentExpression));

            return !assignmentExpression.IsKind(SyntaxKind.SimpleAssignmentExpression)
                && assignmentExpression.Left?.IsMissing == false
                && assignmentExpression.Right?.IsMissing == false;
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

            ExpressionSyntax left = assignmentExpression.Left;

            AssignmentExpressionSyntax newNode = SimpleAssignmentExpression(
                left,
                BinaryExpression(
                    GetBinaryExpressionKind(assignmentExpression),
                    left.WithoutLeadingTrivia(),
                    assignmentExpression.Right));

            newNode = newNode
                .WithTriviaFrom(assignmentExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(assignmentExpression, newNode, cancellationToken);
        }

        private static SyntaxKind GetBinaryExpressionKind(AssignmentExpressionSyntax assignmentExpression)
        {
            switch (assignmentExpression.Kind())
            {
                case SyntaxKind.AddAssignmentExpression:
                    return SyntaxKind.AddExpression;
                case SyntaxKind.SubtractAssignmentExpression:
                    return SyntaxKind.SubtractExpression;
                case SyntaxKind.MultiplyAssignmentExpression:
                    return SyntaxKind.MultiplyExpression;
                case SyntaxKind.DivideAssignmentExpression:
                    return SyntaxKind.DivideExpression;
                case SyntaxKind.ModuloAssignmentExpression:
                    return SyntaxKind.ModuloExpression;
                case SyntaxKind.AndAssignmentExpression:
                    return SyntaxKind.BitwiseAndExpression;
                case SyntaxKind.OrAssignmentExpression:
                    return SyntaxKind.BitwiseOrExpression;
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                    return SyntaxKind.ExclusiveOrExpression;
                case SyntaxKind.LeftShiftAssignmentExpression:
                    return SyntaxKind.LeftShiftExpression;
                case SyntaxKind.RightShiftAssignmentExpression:
                    return SyntaxKind.RightShiftExpression;
            }

            Debug.Assert(false, assignmentExpression.Kind().ToString());
            return SyntaxKind.None;
        }
    }
}
