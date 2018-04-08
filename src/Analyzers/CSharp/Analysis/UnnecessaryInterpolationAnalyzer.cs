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
    public class UnnecessaryInterpolationAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UnnecessaryInterpolation); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeInterpolation, SyntaxKind.Interpolation);
        }

        public static void AnalyzeInterpolation(SyntaxNodeAnalysisContext context)
        {
            var interpolation = (InterpolationSyntax)context.Node;

            StringLiteralExpressionInfo stringLiteralInfo = SyntaxInfo.StringLiteralExpressionInfo(interpolation.Expression);

            if (!stringLiteralInfo.Success)
                return;

            if (!(interpolation.Parent is InterpolatedStringExpressionSyntax interpolatedString))
                return;

            if (interpolatedString.StringStartToken.ValueText.Contains("@") != stringLiteralInfo.IsVerbatim)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryInterpolation, interpolation);
        }
    }
}
