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
            if (!binaryExpression.ContainsDiagnostics
                && !binaryExpression.SpanContainsDirectives())
            {
                ExpressionSyntax left = binaryExpression.Left.WalkDownParentheses();
                ExpressionSyntax right = binaryExpression.Right.WalkDownParentheses();

                switch (binaryExpression.Kind())
                {
                    case SyntaxKind.LogicalOrExpression:
                        {
                            if (left.IsKind(SyntaxKind.EqualsExpression)
                                && right.IsKind(SyntaxKind.EqualsExpression)
                                && CanRefactor(
                                    (BinaryExpressionSyntax)left,
                                    (BinaryExpressionSyntax)right,
                                    context.SemanticModel,
                                    context.CancellationToken))
                            {
                                context.ReportDiagnostic(DiagnosticDescriptors.UseStringIsNullOrEmptyMethod, binaryExpression);
                            }

                            break;
                        }
                    case SyntaxKind.LogicalAndExpression:
                        {
                            if (left.IsKind(SyntaxKind.NotEqualsExpression)
                                && right.IsKind(SyntaxKind.NotEqualsExpression, SyntaxKind.GreaterThanExpression)
                                && CanRefactor(
                                    (BinaryExpressionSyntax)left,
                                    (BinaryExpressionSyntax)right,
                                    context.SemanticModel,
                                    context.CancellationToken))
                            {
                                context.ReportDiagnostic(DiagnosticDescriptors.UseStringIsNullOrEmptyMethod, binaryExpression);
                            }

                            break;
                        }
                }
            }
        }

        private static bool CanRefactor(
            BinaryExpressionSyntax left,
            BinaryExpressionSyntax right,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (left.Right.IsKind(SyntaxKind.NullLiteralExpression))
            {
                ExpressionSyntax rightLeft = right.Left;

                ExpressionSyntax expression = left.Left;

                if (SyntaxComparer.AreEquivalent(expression, rightLeft))
                {
                    if (right.IsKind(SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression)
                        && SymbolEquals(expression, rightLeft, semanticModel, cancellationToken)
                        && CSharpUtility.IsEmptyString(right.Right, semanticModel, cancellationToken))
                    {
                        return true;
                    }
                }
                else if (rightLeft.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                {
                    var memberAccess = (MemberAccessExpressionSyntax)rightLeft;

                    if (string.Equals(memberAccess.Name.Identifier.ValueText, "Length", StringComparison.Ordinal)
                        && right.Right.IsNumericLiteralExpression("0"))
                    {
                        ISymbol symbol = semanticModel.GetSymbol(memberAccess, cancellationToken);

                        if (symbol?.IsProperty() == true)
                        {
                            var propertySymbol = (IPropertySymbol)symbol;
                            if (!propertySymbol.IsIndexer
                                && propertySymbol.IsPublic()
                                && !propertySymbol.IsStatic
                                && propertySymbol.Type.IsInt()
                                && propertySymbol.ContainingType?.IsString() == true
                                && string.Equals(propertySymbol.Name, "Length", StringComparison.Ordinal)
                                && SyntaxComparer.AreEquivalent(expression, memberAccess.Expression)
                                && SymbolEquals(expression, memberAccess.Expression, semanticModel, cancellationToken))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static bool SymbolEquals(
            ExpressionSyntax expression1,
            ExpressionSyntax expression2,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return semanticModel.GetSymbol(expression1, cancellationToken)?
                .Equals(semanticModel.GetSymbol(expression2, cancellationToken)) == true;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left.WalkDownParentheses();
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
