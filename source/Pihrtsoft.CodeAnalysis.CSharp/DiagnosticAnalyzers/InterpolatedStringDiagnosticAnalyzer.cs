// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InterpolatedStringDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UseStringLiteralInsteadOfInterpolatedString,
                    DiagnosticDescriptors.ReplaceInterpolatedStringWithStringLiteralFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.InterpolatedStringExpression);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var interpolatedString = (InterpolatedStringExpressionSyntax)context.Node;

            if (InterpolatedStringRefactoring.CanConvertToStringLiteral(interpolatedString))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseStringLiteralInsteadOfInterpolatedString,
                    context.Node.GetLocation());

                SyntaxToken token = interpolatedString.StringStartToken;

                if (!token.IsMissing
                    && token.Text.StartsWith("$"))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.ReplaceInterpolatedStringWithStringLiteralFadeOut,
                        Location.Create(
                            interpolatedString.SyntaxTree,
                            new TextSpan(token.SpanStart, 1)));
                }
            }
        }
    }
}
