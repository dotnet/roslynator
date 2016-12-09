// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

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

            context.RegisterSyntaxNodeAction(f => AnalyzeBinaryExpression(f),
                SyntaxKind.EqualsExpression,
                SyntaxKind.NotEqualsExpression,
                SyntaxKind.LogicalAndExpression,
                SyntaxKind.LogicalOrExpression);
        }

        private void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax left = binaryExpression.Left;

            if (left?.IsMissing == false)
            {
                ExpressionSyntax right = binaryExpression.Right;

                if (right?.IsMissing == false)
                {
                    switch (binaryExpression.Kind())
                    {
                        case SyntaxKind.EqualsExpression:
                            {
                                AnalyzeEqualsNotEquals(
                                    context,
                                    binaryExpression,
                                    left,
                                    right,
                                    SyntaxKind.FalseLiteralExpression,
                                    SyntaxKind.TrueLiteralExpression);

                                break;
                            }
                        case SyntaxKind.NotEqualsExpression:
                            {
                                AnalyzeEqualsNotEquals(
                                    context,
                                    binaryExpression,
                                    left,
                                    right,
                                    SyntaxKind.TrueLiteralExpression,
                                    SyntaxKind.FalseLiteralExpression);

                                break;
                            }
                        case SyntaxKind.LogicalAndExpression:
                            {
                                AnalyzerLogicalAndLogicalOr(
                                    context,
                                    binaryExpression,
                                    left,
                                    right,
                                    SyntaxKind.TrueLiteralExpression);

                                break;
                            }
                        case SyntaxKind.LogicalOrExpression:
                            {
                                AnalyzerLogicalAndLogicalOr(
                                    context,
                                    binaryExpression,
                                    left,
                                    right,
                                    SyntaxKind.FalseLiteralExpression);

                                break;
                            }
                    }
                }
            }
        }

        private static void AnalyzeEqualsNotEquals(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax left,
            ExpressionSyntax right,
            SyntaxKind kind,
            SyntaxKind kind2)
        {
            SyntaxKind leftKind = left.Kind();

            if (leftKind == kind)
            {
                if (IsBooleanExpressionButNotBooleanLiteral(right, context.SemanticModel, context.CancellationToken))
                    SimplifyBooleanComparisonRefactoring.ReportDiagnostic(context, binaryExpression);
            }
            else if (leftKind == kind2)
            {
                if (IsBooleanExpressionButNotBooleanLiteral(right, context.SemanticModel, context.CancellationToken))
                    RemoveRedundantBooleanLiteralRefactoring.ReportDiagnostic(context, binaryExpression, left);
            }
            else
            {
                SyntaxKind rightKind = right.Kind();

                if (rightKind == kind)
                {
                    if (IsBooleanExpressionButNotBooleanLiteral(left, context.SemanticModel, context.CancellationToken))
                        SimplifyBooleanComparisonRefactoring.ReportDiagnostic(context, binaryExpression);
                }
                else if (rightKind == kind2)
                {
                    if (IsBooleanExpressionButNotBooleanLiteral(left, context.SemanticModel, context.CancellationToken))
                        RemoveRedundantBooleanLiteralRefactoring.ReportDiagnostic(context, binaryExpression, right);
                }
            }
        }

        private static void AnalyzerLogicalAndLogicalOr(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax left,
            ExpressionSyntax right,
            SyntaxKind kind)
        {
            if (left.IsKind(kind))
            {
                RemoveRedundantBooleanLiteralRefactoring.ReportDiagnostic(context, binaryExpression, left);
            }
            else if (right.IsKind(kind))
            {
                RemoveRedundantBooleanLiteralRefactoring.ReportDiagnostic(context, binaryExpression, right);
            }
        }

        private static bool IsBooleanExpressionButNotBooleanLiteral(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    return false;
                case SyntaxKind.LogicalNotExpression:
                    return true;
                default:
                    {
                        return semanticModel
                            .GetTypeInfo(expression, cancellationToken)
                            .ConvertedType?
                            .IsBoolean() == true;
                    }
            }
        }
    }
}
