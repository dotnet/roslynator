// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InterpolatedStringAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AvoidInterpolatedStringWithNoInterpolation); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeInterpolatedStringExpression, SyntaxKind.InterpolatedStringExpression);
        }

        private static void AnalyzeInterpolatedStringExpression(SyntaxNodeAnalysisContext context)
        {
            var interpolatedString = (InterpolatedStringExpressionSyntax)context.Node;

            if (ReplaceInterpolatedStringWithStringLiteralAnalysis.IsFixable(interpolatedString))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AvoidInterpolatedStringWithNoInterpolation,
                    Location.Create(interpolatedString.SyntaxTree, GetDollarSpan(interpolatedString)));
            }
        }

        private static TextSpan GetDollarSpan(InterpolatedStringExpressionSyntax interpolatedString)
        {
            SyntaxToken token = interpolatedString.StringStartToken;

            if (token.Text.StartsWith("$", StringComparison.Ordinal))
                return new TextSpan(token.SpanStart, 1);

            return new TextSpan(token.SpanStart, 0);
        }
    }
}
