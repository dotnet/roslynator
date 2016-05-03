// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;
using static Pihrtsoft.CodeAnalysis.CSharp.DiagnosticHelper;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BooleanComparisonDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveRedundantBooleanComparison,
                    DiagnosticDescriptors.RemoveRedundantBooleanComparisonFadeOut,
                    DiagnosticDescriptors.SimplifyBooleanComparison,
                    DiagnosticDescriptors.SimplifyBooleanComparisonFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeEqualsExpression(f),
                SyntaxKind.EqualsExpression,
                SyntaxKind.NotEqualsExpression);
        }

        private void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            switch (BooleanComparisonAnalysis.Analyze(
                binaryExpression,
                context.SemanticModel,
                context.CancellationToken))
            {
                case BooleanComparisonAnalysisResult.RemoveRedundantBooleanComparison:
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.RemoveRedundantBooleanComparison,
                            binaryExpression.GetLocation());

                        RemoveRedundantBooleanComparisonFadeOut(context, binaryExpression);

                        break;
                    }
                case BooleanComparisonAnalysisResult.SimplifyBooleanComparison:
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.SimplifyBooleanComparison,
                            binaryExpression.GetLocation());

                        SimplifyBooleanComparisonFadeOut(context, binaryExpression);

                        break;
                    }
            }
        }

        private static void RemoveRedundantBooleanComparisonFadeOut(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax binaryExpression)
        {
            DiagnosticDescriptor descriptor = DiagnosticDescriptors.RemoveRedundantBooleanComparisonFadeOut;

            FadeOutToken(context, binaryExpression.OperatorToken, descriptor);

            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            if (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
            {
                if (left.IsKind(SyntaxKind.TrueLiteralExpression))
                {
                    FadeOutNode(context, left, descriptor);
                }
                else if (right.IsKind(SyntaxKind.TrueLiteralExpression))
                {
                    FadeOutNode(context, right, descriptor);
                }
            }
            else if (binaryExpression.IsKind(SyntaxKind.NotEqualsExpression))
            {
                if (left.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    FadeOutNode(context, left, descriptor);
                }
                else if (right.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    FadeOutNode(context, right, descriptor);
                }
            }
        }

        private static void SimplifyBooleanComparisonFadeOut(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax binaryExpression)
        {
            DiagnosticDescriptor descriptor = DiagnosticDescriptors.SimplifyBooleanComparisonFadeOut;

            FadeOutToken(context, binaryExpression.OperatorToken, descriptor);

            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            if (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
            {
                if (left.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    FadeOutNode(context, left, descriptor);

                    if (right.IsKind(SyntaxKind.LogicalNotExpression))
                        FadeOutToken(context, ((PrefixUnaryExpressionSyntax)right).OperatorToken, descriptor);
                }
                else if (right.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    FadeOutNode(context, right, descriptor);

                    if (left.IsKind(SyntaxKind.LogicalNotExpression))
                        FadeOutToken(context, ((PrefixUnaryExpressionSyntax)left).OperatorToken, descriptor);
                }
            }
            else if (binaryExpression.IsKind(SyntaxKind.NotEqualsExpression))
            {
                if (left.IsKind(SyntaxKind.TrueLiteralExpression))
                {
                    FadeOutNode(context, left, descriptor);

                    if (right.IsKind(SyntaxKind.LogicalNotExpression))
                        FadeOutToken(context, ((PrefixUnaryExpressionSyntax)right).OperatorToken, descriptor);
                }
                else if (right.IsKind(SyntaxKind.TrueLiteralExpression))
                {
                    FadeOutNode(context, right, descriptor);

                    if (left.IsKind(SyntaxKind.LogicalNotExpression))
                        FadeOutToken(context, ((PrefixUnaryExpressionSyntax)left).OperatorToken, descriptor);
                }
            }
        }
    }
}
