// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class AssignmentExpressionSyntaxExtensions
    {
        public static AssignmentExpressionSyntax Expand(this AssignmentExpressionSyntax assignmentExpression)
        {
            if (assignmentExpression == null)
                throw new ArgumentNullException(nameof(assignmentExpression));

            if (assignmentExpression.IsKind(SyntaxKind.SimpleAssignmentExpression))
                return assignmentExpression;

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
                default:
                    return SyntaxKind.None;
            }
        }
    }
}
