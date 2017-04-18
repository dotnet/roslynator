// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidNullLiteralExpressionOnLeftSideOfBinaryExpressionRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax binaryExpression)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            if (left?.IsMissing == false
                && right?.IsMissing == false
                && binaryExpression.IsKind(SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression)
                && left.IsKind(SyntaxKind.NullLiteralExpression)
                && !right.IsKind(SyntaxKind.NullLiteralExpression)
                && !binaryExpression.SpanContainsDirectives())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AvoidNullLiteralExpressionOnLeftSideOfBinaryExpression,
                    left);
            }
        }
    }
}
