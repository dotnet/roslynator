// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FormatBinaryOperatorOnNextLineAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.FormatBinaryOperatorOnNextLine); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(
                AnalyzeBinaryExpression,
                SyntaxKind.AddExpression,
                SyntaxKind.SubtractExpression,
                SyntaxKind.MultiplyExpression,
                SyntaxKind.DivideExpression,
                SyntaxKind.ModuloExpression,
                SyntaxKind.LeftShiftExpression,
                SyntaxKind.RightShiftExpression,
                SyntaxKind.LogicalOrExpression,
                SyntaxKind.LogicalAndExpression,
                SyntaxKind.BitwiseOrExpression,
                SyntaxKind.BitwiseAndExpression,
                SyntaxKind.ExclusiveOrExpression,
                SyntaxKind.EqualsExpression,
                SyntaxKind.NotEqualsExpression,
                SyntaxKind.LessThanExpression,
                SyntaxKind.LessThanOrEqualExpression,
                SyntaxKind.GreaterThanExpression,
                SyntaxKind.GreaterThanOrEqualExpression,
                SyntaxKind.IsExpression,
                SyntaxKind.AsExpression);
        }

        public static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
        {
            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            BinaryExpressionInfo info = SyntaxInfo.BinaryExpressionInfo(binaryExpression);

            if (!info.Success)
                return;

            if (CSharpUtility.IsStringConcatenation(binaryExpression, context.SemanticModel, context.CancellationToken))
                return;

            if (!info.Left.GetTrailingTrivia().All(f => f.IsWhitespaceTrivia()))
                return;

            if (!CheckOperatorTrailingTrivia(binaryExpression.OperatorToken.TrailingTrivia))
                return;

            if (!info.Right.GetLeadingTrivia().IsEmptyOrWhitespace())
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.FormatBinaryOperatorOnNextLine,
                binaryExpression.OperatorToken);
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
    }
}
