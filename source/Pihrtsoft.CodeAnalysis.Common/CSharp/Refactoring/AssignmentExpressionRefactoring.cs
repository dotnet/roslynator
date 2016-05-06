// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class AssignmentExpressionRefactoring
    {
        public static bool CanExpand(AssignmentExpressionSyntax assignmentExpression)
        {
            if (assignmentExpression == null)
                throw new ArgumentNullException(nameof(assignmentExpression));

            return !assignmentExpression.IsKind(SyntaxKind.SimpleAssignmentExpression)
                && assignmentExpression.Left != null
                && !assignmentExpression.Left.IsMissing
                && assignmentExpression.Right != null
                && !assignmentExpression.Right.IsMissing;
        }

        public static async Task<Document> ExpandAsync(
            Document document,
            AssignmentExpressionSyntax assignmentExpression,
            CancellationToken cancellationToken)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (assignmentExpression == null)
                throw new ArgumentNullException(nameof(assignmentExpression));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            AssignmentExpressionSyntax newAssignmentExpression = Expand(assignmentExpression)
                .WithTriviaFrom(assignmentExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(assignmentExpression, newAssignmentExpression);

            return document.WithSyntaxRoot(newRoot);
        }

        public static AssignmentExpressionSyntax Expand(this AssignmentExpressionSyntax assignmentExpression)
        {
            if (assignmentExpression == null)
                throw new ArgumentNullException(nameof(assignmentExpression));

            return
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    assignmentExpression.Left,
                    BinaryExpression(
                        GetBinaryExpressionKind(assignmentExpression),
                        assignmentExpression.Left.WithoutLeadingTrivia(),
                        assignmentExpression.Right));
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

        public static AssignmentExpressionSyntax Simplify(AssignmentExpressionSyntax assignmentExpression)
        {
            if (assignmentExpression == null)
                throw new ArgumentNullException(nameof(assignmentExpression));

            var binaryExpression = (BinaryExpressionSyntax)assignmentExpression.Right;

            return AssignmentExpression(
                GetAssignmentExpressionKind(binaryExpression),
                assignmentExpression.Left,
                binaryExpression.Right);
        }

        private static SyntaxKind GetAssignmentExpressionKind(BinaryExpressionSyntax binaryExpression)
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
            }

            Debug.Assert(false, binaryExpression.Kind().ToString());
            return SyntaxKind.None;
        }
    }
}
