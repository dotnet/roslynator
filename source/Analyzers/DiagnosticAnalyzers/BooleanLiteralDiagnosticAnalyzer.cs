// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BooleanLiteralDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveRedundantBooleanLiteral,
                    DiagnosticDescriptors.RemoveRedundantBooleanLiteralFadeOut,
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
                SyntaxKind.NotEqualsExpression,
                SyntaxKind.LogicalAndExpression,
                SyntaxKind.LogicalOrExpression);
        }

        private void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            BinaryExpressionAnalysisResult result = BinaryExpressionAnalysis.Analyze(
                binaryExpression,
                context.SemanticModel,
                context.CancellationToken);

            if (result == BinaryExpressionAnalysisResult.RemoveRedundantBooleanLiteral)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveRedundantBooleanLiteral,
                    binaryExpression.GetLocation());

                RemoveRedundantBooleanLiteralFadeOut(context, binaryExpression);
            }
            else if (result == BinaryExpressionAnalysisResult.SimplifyBooleanComparison)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.SimplifyBooleanComparison,
                    binaryExpression.GetLocation());

                SimplifyBooleanComparisonFadeOut(context, binaryExpression);
            }
        }

        private static void RemoveRedundantBooleanLiteralFadeOut(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax binaryExpression)
        {
            DiagnosticDescriptor descriptor = DiagnosticDescriptors.RemoveRedundantBooleanLiteralFadeOut;

            context.FadeOutToken(descriptor, binaryExpression.OperatorToken);

            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            switch (binaryExpression.Kind())
            {
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.LogicalAndExpression:
                    {
                        if (left.IsKind(SyntaxKind.TrueLiteralExpression))
                        {
                            context.FadeOutNode(descriptor, left);
                        }
                        else if (right.IsKind(SyntaxKind.TrueLiteralExpression))
                        {
                            context.FadeOutNode(descriptor, right);
                        }

                        break;
                    }
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.LogicalOrExpression:
                    {
                        if (left.IsKind(SyntaxKind.FalseLiteralExpression))
                        {
                            context.FadeOutNode(descriptor, left);
                        }
                        else if (right.IsKind(SyntaxKind.FalseLiteralExpression))
                        {
                            context.FadeOutNode(descriptor, right);
                        }

                        break;
                    }
            }
        }

        private static void SimplifyBooleanComparisonFadeOut(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax binaryExpression)
        {
            DiagnosticDescriptor descriptor = DiagnosticDescriptors.SimplifyBooleanComparisonFadeOut;

            context.FadeOutToken(descriptor, binaryExpression.OperatorToken);

            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            if (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
            {
                if (left.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    context.FadeOutNode(descriptor, left);

                    if (right.IsKind(SyntaxKind.LogicalNotExpression))
                        context.FadeOutToken(descriptor, ((PrefixUnaryExpressionSyntax)right).OperatorToken);
                }
                else if (right.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    context.FadeOutNode(descriptor, right);

                    if (left.IsKind(SyntaxKind.LogicalNotExpression))
                        context.FadeOutToken(descriptor, ((PrefixUnaryExpressionSyntax)left).OperatorToken);
                }
            }
            else if (binaryExpression.IsKind(SyntaxKind.NotEqualsExpression))
            {
                if (left.IsKind(SyntaxKind.TrueLiteralExpression))
                {
                    context.FadeOutNode(descriptor, left);

                    if (right.IsKind(SyntaxKind.LogicalNotExpression))
                        context.FadeOutToken(descriptor, ((PrefixUnaryExpressionSyntax)right).OperatorToken);
                }
                else if (right.IsKind(SyntaxKind.TrueLiteralExpression))
                {
                    context.FadeOutNode(descriptor, right);

                    if (left.IsKind(SyntaxKind.LogicalNotExpression))
                        context.FadeOutToken(descriptor, ((PrefixUnaryExpressionSyntax)left).OperatorToken);
                }
            }
        }
    }
}
