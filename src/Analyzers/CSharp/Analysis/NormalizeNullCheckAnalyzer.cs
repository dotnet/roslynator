// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class NormalizeNullCheckAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.NormalizeNullCheck);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(
                c =>
                {
                    if (c.GetNullCheckStyle() == NullCheckStyle.EqualityOperator)
                        AnalyzeIsPatternExpression(c);
                },
                SyntaxKind.IsPatternExpression);

            context.RegisterSyntaxNodeAction(
                c =>
                {
                    if (c.GetNullCheckStyle() == NullCheckStyle.PatternMatching)
                        AnalyzeEqualsExpression(c);
                },
                SyntaxKind.EqualsExpression);

            context.RegisterSyntaxNodeAction(
                c =>
                {
                    if (((CSharpCompilation)c.Compilation).LanguageVersion >= LanguageVersion.CSharp9
                        && c.GetNullCheckStyle() == NullCheckStyle.PatternMatching)
                    {
                        AnalyzeNotEqualsExpression(c);
                    }
                },
                SyntaxKind.NotEqualsExpression);
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
                    DiagnosticRules.NormalizeNullCheck,
                    binaryExpression,
                    "pattern matching");
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
                    DiagnosticRules.NormalizeNullCheck,
                    binaryExpression,
                    "pattern matching");
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
                    DiagnosticRules.NormalizeNullCheck,
                    isPatternExpression,
                    "equality operator");
            }
        }
    }
}
