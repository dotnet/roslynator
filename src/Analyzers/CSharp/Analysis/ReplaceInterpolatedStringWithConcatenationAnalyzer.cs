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
    public class ReplaceInterpolatedStringWithConcatenationAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.ReplaceInterpolatedStringWithConcatenation,
                    DiagnosticDescriptors.ReplaceInterpolatedStringWithConcatenationFadeOut);
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

            if (contents.Count <= 1)
                return;

            if (contents.Any(f => f.Kind() != SyntaxKind.Interpolation))
                return;

            foreach (InterpolatedStringContentSyntax content in contents)
            {
                var interpolation = (InterpolationSyntax)content;

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
            }

            context.ReportDiagnostic(DiagnosticDescriptors.ReplaceInterpolatedStringWithConcatenation, interpolatedString);

            context.ReportToken(DiagnosticDescriptors.ReplaceInterpolatedStringWithConcatenationFadeOut, interpolatedString.StringStartToken);

            foreach (InterpolatedStringContentSyntax content in contents)
            {
                var interpolation = (InterpolationSyntax)content;

                context.ReportToken(DiagnosticDescriptors.ReplaceInterpolatedStringWithConcatenationFadeOut, interpolation.OpenBraceToken);
                context.ReportToken(DiagnosticDescriptors.ReplaceInterpolatedStringWithConcatenationFadeOut, interpolation.CloseBraceToken);
            }

            context.ReportToken(DiagnosticDescriptors.ReplaceInterpolatedStringWithConcatenationFadeOut, interpolatedString.StringEndToken);
        }
    }
}
