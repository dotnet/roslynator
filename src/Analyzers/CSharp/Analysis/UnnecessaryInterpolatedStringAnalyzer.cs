// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnnecessaryInterpolatedStringAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UnnecessaryInterpolatedString,
                    DiagnosticDescriptors.UnnecessaryInterpolatedStringFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (startContext.IsAnalyzerSuppressed(DiagnosticDescriptors.UnnecessaryInterpolatedString))
                    return;

                startContext.RegisterSyntaxNodeAction(AnalyzeInterpolatedStringExpression, SyntaxKind.InterpolatedStringExpression);
            });
        }

        public static void AnalyzeInterpolatedStringExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            if (context.Node.ContainsDirectives)
                return;

            var interpolatedString = (InterpolatedStringExpressionSyntax)context.Node;

            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            if (!(contents.SingleOrDefault(shouldThrow: false) is InterpolationSyntax interpolation))
                return;

            if (interpolation.AlignmentClause != null)
                return;

            if (interpolation.FormatClause != null)
                return;

            ExpressionSyntax expression = interpolation.Expression?.WalkDownParentheses();

            if (expression == null)
                return;

            bool isNonNullStringExpression = false;

            if (expression.IsKind(SyntaxKind.StringLiteralExpression, SyntaxKind.InterpolatedStringExpression))
            {
                isNonNullStringExpression = true;
            }
            else
            {
                Optional<object> constantValue = context.SemanticModel.GetConstantValue(expression, context.CancellationToken);

                if (constantValue.HasValue
                    && constantValue.Value is string s
                    && s != null)
                {
                    isNonNullStringExpression = true;
                }
            }

            if (!isNonNullStringExpression)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryInterpolatedString, interpolatedString);

            context.ReportToken(DiagnosticDescriptors.UnnecessaryInterpolatedStringFadeOut, interpolatedString.StringStartToken);
            context.ReportToken(DiagnosticDescriptors.UnnecessaryInterpolatedStringFadeOut, interpolation.OpenBraceToken);
            context.ReportToken(DiagnosticDescriptors.UnnecessaryInterpolatedStringFadeOut, interpolation.CloseBraceToken);
            context.ReportToken(DiagnosticDescriptors.UnnecessaryInterpolatedStringFadeOut, interpolatedString.StringEndToken);
        }
    }
}
