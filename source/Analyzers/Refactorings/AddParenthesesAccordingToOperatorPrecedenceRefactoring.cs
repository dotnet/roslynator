// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddParenthesesAccordingToOperatorPrecedenceRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax binaryExpression)
        {
            SyntaxKind kind = binaryExpression.Kind();

            Analyze(context, kind, binaryExpression.Left);
            Analyze(context, kind, binaryExpression.Right);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxKind kind, ExpressionSyntax expression)
        {
            if (expression != null
                && GetGroupNumber(kind) == GetGroupNumber(expression.Kind())
                && CSharpAnalysis.GetOperatorPrecedence(expression) < CSharpAnalysis.GetOperatorPrecedence(kind))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddParenthesesAccordingToOperatorPrecedence,
                    expression.GetLocation());
            }
        }

        private static int GetGroupNumber(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                    return 1;
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                    return 2;
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                    return 3;
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    return 4;
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.LogicalOrExpression:
                    return 5;
                default:
                    return 0;
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ParenthesizedExpressionSyntax newNode = expression.Parenthesize(moveTrivia: true);

            return document.ReplaceNodeAsync(expression, newNode, cancellationToken);
        }
    }
}