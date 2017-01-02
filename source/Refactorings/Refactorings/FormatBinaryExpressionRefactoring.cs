// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.CSharp.Formatting;
using Roslynator.Text.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatBinaryExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            binaryExpression = GetBinaryExpression(binaryExpression, context.Span);

            if (binaryExpression != null
                && IsFormattableKind(binaryExpression.Kind()))
            {
                string title = "Format binary expression";

                if (binaryExpression.IsSingleLine())
                {
                    title += " on multiple lines";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken => CSharpFormatter.ToMultiLineAsync(context.Document, binaryExpression, cancellationToken));
                }
                else
                {
                    title += " on a single line";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken => CSharpFormatter.ToSingleLineAsync(context.Document, binaryExpression, cancellationToken));
                }
            }
        }

        private static BinaryExpressionSyntax GetBinaryExpression(BinaryExpressionSyntax binaryExpression, TextSpan span)
        {
            if (span.IsEmpty)
            {
                return GetTopmostBinaryExpression(binaryExpression);
            }
            else if (span.IsBetweenSpans(binaryExpression)
                && binaryExpression == GetTopmostBinaryExpression(binaryExpression))
            {
                return binaryExpression;
            }

            return null;
        }

        private static bool IsFormattableKind(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.BitwiseOrExpression:
                    return true;
                default:
                    return false;
            }
        }

        private static BinaryExpressionSyntax GetTopmostBinaryExpression(BinaryExpressionSyntax binaryExpression)
        {
            bool success = true;

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
