// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseIsOperatorInsteadOfAsOperatorRefactoring
    {
        public static void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            Analyze(context, (BinaryExpressionSyntax)context.Node);
        }

        public static void AnalyzeNotEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            Analyze(context, (BinaryExpressionSyntax)context.Node);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax binaryExpression)
        {
            ExpressionSyntax right = binaryExpression.Right;

            if (right?.IsKind(SyntaxKind.NullLiteralExpression) == true)
            {
                ExpressionSyntax left = binaryExpression.Left;

                if (left != null)
                {
                    left = left.WalkDownParentheses();

                    if (left?.IsKind(SyntaxKind.AsExpression) == true)
                    {
                        var asExpression = (BinaryExpressionSyntax)left;

                        if (asExpression.Left?.IsMissing == false
                            && asExpression.Right is TypeSyntax
                            && !binaryExpression.SpanContainsDirectives())
                        {
                            context.ReportDiagnostic(DiagnosticDescriptors.UseIsOperatorInsteadOfAsOperator, binaryExpression);
                        }
                    }
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            var asExpression = (BinaryExpressionSyntax)binaryExpression.Left.WalkDownParentheses();

            ExpressionSyntax newNode = IsExpression(asExpression.Left, (TypeSyntax)asExpression.Right);

            if (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
                newNode = LogicalNotExpression(newNode.WithoutTrivia().Parenthesize()).WithTriviaFrom(newNode);

            newNode = newNode
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }
    }
}
