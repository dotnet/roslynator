// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class WrapBinaryExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            binaryExpression = GetBinaryExpression(binaryExpression, context.Span);

            if (binaryExpression == null)
                return;

            if (!IsFormattableKind(binaryExpression.Kind()))
                return;

            if (binaryExpression.IsSingleLine())
            {
                context.RegisterRefactoring(
                    "Wrap binary expression",
                    ct => SyntaxFormatter.WrapBinaryExpressionAsync(context.Document, binaryExpression, ct),
                    RefactoringIdentifiers.WrapBinaryExpression);
            }
            else if (binaryExpression.DescendantTrivia(binaryExpression.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.RegisterRefactoring(
                    "Unwrap binary expression",
                    ct => SyntaxFormatter.UnwrapExpressionAsync(context.Document, binaryExpression, ct),
                    RefactoringIdentifiers.WrapBinaryExpression);
            }
        }

        private static BinaryExpressionSyntax GetBinaryExpression(BinaryExpressionSyntax binaryExpression, TextSpan span)
        {
            if (span.IsEmpty)
                return GetTopmostBinaryExpression(binaryExpression);

            if (span.IsBetweenSpans(binaryExpression)
                && binaryExpression == GetTopmostBinaryExpression(binaryExpression))
            {
                return binaryExpression;
            }

            return null;
        }

        private static bool IsFormattableKind(SyntaxKind kind)
        {
            return kind.Is(
                SyntaxKind.LogicalAndExpression,
                SyntaxKind.LogicalOrExpression,
                SyntaxKind.BitwiseAndExpression,
                SyntaxKind.BitwiseOrExpression);
        }

        private static BinaryExpressionSyntax GetTopmostBinaryExpression(BinaryExpressionSyntax binaryExpression)
        {
            var success = true;

            while (success)
            {
                success = false;

                if (binaryExpression.Parent != null
                    && IsFormattableKind(binaryExpression.Parent.Kind()))
                {
                    var parent = (BinaryExpressionSyntax)binaryExpression.Parent;

                    if (parent.Left?.IsMissing == false
                        && parent.Right?.IsMissing == false)
                    {
                        binaryExpression = parent;
                        success = true;
                    }
                }
            }

            return binaryExpression;
        }
    }
}
