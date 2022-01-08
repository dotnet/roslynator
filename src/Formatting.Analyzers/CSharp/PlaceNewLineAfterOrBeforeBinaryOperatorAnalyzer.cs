// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class PlaceNewLineAfterOrBeforeBinaryOperatorAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.PlaceNewLineAfterOrBeforeBinaryOperator);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(
                f => AnalyzeBinaryExpression(f),
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

        private static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
        {
            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax left = binaryExpression.Left;

            if (left.IsMissing)
                return;

            ExpressionSyntax right = binaryExpression.Right;

            if (right.IsMissing)
                return;

            NewLinePosition newLinePosition = context.GetBinaryExpressionNewLinePosition();

            if (newLinePosition == NewLinePosition.None)
                return;

            if (SyntaxTriviaAnalysis.IsTokenFollowedWithNewLineAndNotPrecededWithNewLine(left, binaryExpression.OperatorToken, right))
            {
                if (newLinePosition == NewLinePosition.Before)
                {
                    DiagnosticHelpers.ReportDiagnostic(
                        context,
                        DiagnosticRules.PlaceNewLineAfterOrBeforeBinaryOperator,
                        Location.Create(binaryExpression.SyntaxTree, binaryExpression.OperatorToken.Span.WithLength(0)),
                        "before");
                }
            }
            else if (SyntaxTriviaAnalysis.IsTokenPrecededWithNewLineAndNotFollowedWithNewLine(left, binaryExpression.OperatorToken, right)
                && newLinePosition == NewLinePosition.After)
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.PlaceNewLineAfterOrBeforeBinaryOperator,
                    Location.Create(binaryExpression.SyntaxTree, binaryExpression.OperatorToken.Span.WithLength(0)),
                    properties: DiagnosticProperties.AnalyzerOption_Invert,
                    "after");
            }
        }
    }
}
