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

            context.RegisterSyntaxNodeAction(AnalyzeInterpolatedStringExpression, SyntaxKind.InterpolatedStringExpression);
        }

        public static void AnalyzeInterpolatedStringExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            if (context.Node.ContainsDirectives)
                return;

            var interpolatedString = (InterpolatedStringExpressionSyntax)context.Node;

            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            if (!contents.Any())
                return;

            var interpolation = contents.SingleOrDefault(shouldThrow: false) as InterpolationSyntax;

            if (interpolation == null)
                return;

            ExpressionSyntax expression = interpolation.Expression;

            if (expression == null)
                return;

            if (interpolation.AlignmentClause != null)
                return;

            if (interpolation.FormatClause != null)
                return;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if (typeSymbol?.SpecialType != SpecialType.System_String)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryInterpolatedString, interpolatedString);

            context.ReportToken(DiagnosticDescriptors.UnnecessaryInterpolatedStringFadeOut, interpolatedString.StringStartToken);
            context.ReportToken(DiagnosticDescriptors.UnnecessaryInterpolatedStringFadeOut, interpolation.OpenBraceToken);
            context.ReportToken(DiagnosticDescriptors.UnnecessaryInterpolatedStringFadeOut, interpolation.CloseBraceToken);
            context.ReportToken(DiagnosticDescriptors.UnnecessaryInterpolatedStringFadeOut, interpolatedString.StringEndToken);
        }
    }
}
