// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatBinaryOperatorOnNextLineRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax binaryExpression)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            if (left?.IsMissing == false
                && right?.IsMissing == false
                && !IsStringConcatenation(context, binaryExpression)
                && left.GetTrailingTrivia().All(f => f.IsWhitespaceTrivia())
                && CheckOperatorTrailingTrivia(binaryExpression.OperatorToken.TrailingTrivia)
                && right.GetLeadingTrivia().IsEmptyOrWhitespace())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.FormatBinaryOperatorOnNextLine,
                    binaryExpression.OperatorToken);
            }
        }

        private static bool IsStringConcatenation(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax binaryExpression)
        {
            return binaryExpression.IsKind(SyntaxKind.AddExpression)
                && (IsStringExpression(context, binaryExpression.Left) || IsStringExpression(context, binaryExpression.Right));
        }

        private static bool IsStringExpression(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            if (expression.IsKind(SyntaxKind.StringLiteralExpression)
                || expression.IsKind(SyntaxKind.InterpolatedStringExpression))
            {
                return true;
            }

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken).ConvertedType;

            return typeSymbol?.IsNamedType() == true
                && ((INamedTypeSymbol)typeSymbol).IsString();
        }

        private static bool CheckOperatorTrailingTrivia(SyntaxTriviaList triviaList)
        {
            bool result = false;

            foreach (SyntaxTrivia trivia in triviaList)
            {
                switch (trivia.Kind())
                {
                    case SyntaxKind.WhitespaceTrivia:
                        {
                            continue;
                        }
                    case SyntaxKind.EndOfLineTrivia:
                        {
                            result = true;
                            continue;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }

            return result;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            BinaryExpressionSyntax newBinaryExpression = GetNewBinaryExpression(binaryExpression)
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newBinaryExpression, cancellationToken);
        }

        private static BinaryExpressionSyntax GetNewBinaryExpression(BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression == null)
                throw new ArgumentNullException(nameof(binaryExpression));

            return BinaryExpression(
                binaryExpression.Kind(),
                binaryExpression.Left.WithTrailingTrivia(binaryExpression.OperatorToken.TrailingTrivia),
                Token(
                    binaryExpression.Right.GetLeadingTrivia(),
                    binaryExpression.OperatorToken.Kind(),
                    TriviaList(Space)),
                binaryExpression.Right.WithoutLeadingTrivia());
        }
    }
}
