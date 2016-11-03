// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class SimplifyAssignmentExpressionRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            AssignmentExpressionSyntax assignmentExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (assignmentExpression == null)
                throw new ArgumentNullException(nameof(assignmentExpression));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            AssignmentExpressionSyntax newNode = Refactor(assignmentExpression)
                .WithTriviaFrom(assignmentExpression)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(assignmentExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static AssignmentExpressionSyntax Refactor(AssignmentExpressionSyntax assignmentExpression)
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
