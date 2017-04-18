// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseStringIsNullOrEmptyMethodRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax binaryExpression)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            if (left?.IsMissing == false
                && right?.IsMissing == false
                && CanRefactor(binaryExpression, left, right, context.SemanticModel, context.CancellationToken)
                && !binaryExpression.SpanContainsDirectives())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseStringIsNullOrEmptyMethod,
                    binaryExpression);
            }
        }

        public static bool CanRefactor(
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax left,
            ExpressionSyntax right,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (binaryExpression.Kind())
            {
                case SyntaxKind.LogicalOrExpression:
                    {
                        if (left.IsKind(SyntaxKind.EqualsExpression) && right.IsKind(SyntaxKind.EqualsExpression))
                        {
                            return CanRefactor(
                                (BinaryExpressionSyntax)left,
                                (BinaryExpressionSyntax)right,
                                semanticModel,
                                cancellationToken);
                        }

                        break;
                    }
                case SyntaxKind.LogicalAndExpression:
                    {
                        if (left.IsKind(SyntaxKind.NotEqualsExpression) && right.IsKind(SyntaxKind.NotEqualsExpression, SyntaxKind.GreaterThanExpression))
                        {
                            return CanRefactor(
                                (BinaryExpressionSyntax)left,
                                (BinaryExpressionSyntax)right,
                                semanticModel,
                                cancellationToken);
                        }

                        break;
                    }
            }

            return false;
        }

        private static bool CanRefactor(
            BinaryExpressionSyntax left,
            BinaryExpressionSyntax right,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (left.Right?.IsKind(SyntaxKind.NullLiteralExpression) == true
                && right.Right?.IsNumericLiteralExpression(0) == true)
            {
                ExpressionSyntax rightLeft = right.Left;

                if (rightLeft?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)rightLeft;

                    if (string.Equals(memberAccess.Name?.Identifier.ValueText, "Length", StringComparison.Ordinal))
                    {
                        ExpressionSyntax expression = left.Left;

                        if (expression != null
                            && semanticModel.GetTypeSymbol(expression, cancellationToken)?.IsString() == true)
                        {
                            ExpressionSyntax expression2 = memberAccess.Expression;

                            if (expression2 != null
                                && semanticModel.GetTypeSymbol(expression2, cancellationToken)?.IsString() == true
                                && expression.IsEquivalentTo(expression2, topLevel: false))
                            {
                                ISymbol symbol = semanticModel.GetSymbol(memberAccess.Name, cancellationToken);

                                if (symbol.IsPublic()
                                    && !symbol.IsStatic
                                    && symbol.IsProperty()
                                    && symbol.Name.Equals("Length", StringComparison.Ordinal))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax expression = ((BinaryExpressionSyntax)left).Left;

            ExpressionSyntax newNode = SimpleMemberInvocationExpression(
                StringType(),
                IdentifierName("IsNullOrEmpty"),
                Argument(expression));

            if (left.IsKind(SyntaxKind.NotEqualsExpression))
                newNode = LogicalNotExpression(newNode);

            newNode = newNode
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }
    }
}
