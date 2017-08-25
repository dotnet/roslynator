// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyCoalesceExpressionRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax coalesceExpression)
        {
            ExpressionSyntax left = coalesceExpression.Left;
            ExpressionSyntax right = coalesceExpression.Right;

            if (left != null
                && right != null)
            {
                SemanticModel semanticModel = context.SemanticModel;
                CancellationToken cancellationToken = context.CancellationToken;

                TextSpan span = GetRedundantSpan(coalesceExpression, left, right, semanticModel, cancellationToken);

                if (span != default(TextSpan)
                    && !coalesceExpression.SpanContainsDirectives())
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.SimplifyCoalesceExpression,
                        Location.Create(coalesceExpression.SyntaxTree, span));
                }
            }
        }

        private static TextSpan GetRedundantSpan(
            BinaryExpressionSyntax coalesceExpression,
            ExpressionSyntax left,
            ExpressionSyntax right,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            switch (GetRedundantPart(left, right, semanticModel, cancellationToken))
            {
                case BinaryExpressionPart.Left:
                    return TextSpan.FromBounds(left.SpanStart, coalesceExpression.OperatorToken.Span.End);
                case BinaryExpressionPart.Right:
                    return TextSpan.FromBounds(coalesceExpression.OperatorToken.SpanStart, coalesceExpression.Right.Span.End);
                default:
                    return default(TextSpan);
            }
        }

        private static BinaryExpressionPart GetRedundantPart(
            ExpressionSyntax left,
            ExpressionSyntax right,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SyntaxKind leftKind = left.Kind();
            SyntaxKind rightKind = right.Kind();

            switch (leftKind)
            {
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.AnonymousObjectCreationExpression:
                case SyntaxKind.ArrayCreationExpression:
                case SyntaxKind.ImplicitArrayCreationExpression:
                case SyntaxKind.InterpolatedStringExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.TypeOfExpression:
                    return BinaryExpressionPart.Right;
                case SyntaxKind.NullLiteralExpression:
                    return BinaryExpressionPart.Left;
                case SyntaxKind.DefaultExpression:
                    {
                        if (IsDefaultOfReferenceType((DefaultExpressionSyntax)left, semanticModel, cancellationToken))
                            return BinaryExpressionPart.Left;

                        break;
                    }
            }

            Optional<object> optional = semanticModel.GetConstantValue(left, cancellationToken);

            if (optional.HasValue)
            {
                object value = optional.Value;

                if (value != null)
                {
                    return BinaryExpressionPart.Right;
                }
                else
                {
                    return BinaryExpressionPart.Left;
                }
            }

            ITypeSymbol leftSymbol = semanticModel.GetTypeSymbol(left, cancellationToken);

            if (leftSymbol?.IsErrorType() == false
                && leftSymbol.IsValueType
                && !leftSymbol.IsConstructedFrom(SpecialType.System_Nullable_T))
            {
                return  BinaryExpressionPart.Right;
            }

            switch (rightKind)
            {
                case SyntaxKind.NullLiteralExpression:
                    return BinaryExpressionPart.Right;
                case SyntaxKind.DefaultExpression:
                    {
                        if (IsDefaultOfReferenceType((DefaultExpressionSyntax)right, semanticModel, cancellationToken))
                            return BinaryExpressionPart.Right;

                        break;
                    }
            }

            if (leftKind == rightKind
                && SyntaxComparer.AreEquivalent(left, right))
            {
                return BinaryExpressionPart.Right;
            }

            return BinaryExpressionPart.None;
        }

        private static bool IsDefaultOfReferenceType(DefaultExpressionSyntax defaultExpression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            TypeSyntax type = defaultExpression.Type;

            if (type != null)
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

                if (typeSymbol?.IsErrorType() == false
                    && typeSymbol.IsReferenceType)
                {
                    return true;
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;
            SyntaxToken operatorToken = binaryExpression.OperatorToken;

            ExpressionSyntax newNode = null;

            if (expression == left)
            {
                IEnumerable<SyntaxTrivia> trivia = binaryExpression.DescendantTrivia(TextSpan.FromBounds(left.FullSpan.Start, operatorToken.FullSpan.End));

                newNode = right.WithLeadingTrivia(trivia);
            }
            else
            {
                IEnumerable<SyntaxTrivia> trivia = binaryExpression.DescendantTrivia(TextSpan.FromBounds(operatorToken.FullSpan.Start, right.FullSpan.End));

                newNode = left.WithTrailingTrivia(trivia);
            }

            newNode = newNode
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }

        private enum BinaryExpressionPart
        {
            None,
            Left,
            Right
        }
    }
}
