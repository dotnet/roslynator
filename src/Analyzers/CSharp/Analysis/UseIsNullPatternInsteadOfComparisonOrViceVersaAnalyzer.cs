// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseIsNullPatternInsteadOfComparisonOrViceVersaAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseIsNullPatternInsteadOfComparisonOrViceVersa); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (!startContext.IsAnalyzerSuppressed(AnalyzerOptions.UseComparisonInsteadOfIsNullPattern))
                {
                    startContext.RegisterSyntaxNodeAction(f => AnalyzeIsPatternExpression(f), SyntaxKind.IsPatternExpression);
                }
                else
                {
                    startContext.RegisterSyntaxNodeAction(f => AnalyzeEqualsExpression(f), SyntaxKind.EqualsExpression);

                    if (!startContext.IsAnalyzerSuppressed(AnalyzerOptions.UseIsNullPatternInsteadOfInequalityOperator))
                        startContext.RegisterSyntaxNodeAction(f => AnalyzeNotEqualsExpression(f), SyntaxKind.NotEqualsExpression);
                }
            });
        }

        private static void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(binaryExpression, allowedStyles: NullCheckStyles.EqualsToNull, walkDownParentheses: false);

            if (nullCheck.Success)
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticDescriptors.UseIsNullPatternInsteadOfComparisonOrViceVersa,
                    binaryExpression,
                    "==");
            }
        }

        private static void AnalyzeNotEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(binaryExpression, allowedStyles: NullCheckStyles.NotEqualsToNull);

            if (nullCheck.Success)
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticDescriptors.UseIsNullPatternInsteadOfComparisonOrViceVersa,
                    binaryExpression,
                    "!=");
            }
        }

        private static void AnalyzeIsPatternExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            var isPatternExpression = (IsPatternExpressionSyntax)context.Node;

            if (isPatternExpression.Pattern is ConstantPatternSyntax constantPattern
                && constantPattern.Expression.IsKind(SyntaxKind.NullLiteralExpression))
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticDescriptors.ReportOnly.UseComparisonInsteadOfIsNullPattern,
                    isPatternExpression);
            }
        }
    }
}
