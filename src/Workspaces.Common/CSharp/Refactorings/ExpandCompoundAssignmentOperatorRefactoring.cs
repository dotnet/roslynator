// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    internal static class ExpandCompoundAssignmentOperatorRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            AssignmentExpressionSyntax assignmentExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
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

            Debug.Fail(assignmentExpression.Kind().ToString());
            return SyntaxKind.None;
        }
    }
}
