// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseIsOperatorInsteadOfAsOperatorAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseIsOperatorInsteadOfAsOperator); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeEqualsExpression(f), SyntaxKind.EqualsExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeNotEqualsExpression(f), SyntaxKind.NotEqualsExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeIsPatternExpression(f), SyntaxKind.IsPatternExpression);
        }

        private static void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            Analyze(context, context.Node);
        }

        private static void AnalyzeNotEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            Analyze(context, context.Node);
        }

        private static void AnalyzeIsPatternExpression(SyntaxNodeAnalysisContext context)
        {
            Analyze(context, context.Node);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            if (node.SpanContainsDirectives())
                return;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(node);

            if (!nullCheck.Success)
                return;

            AsExpressionInfo asExpressionInfo = SyntaxInfo.AsExpressionInfo(nullCheck.Expression);

            if (!asExpressionInfo.Success)
                return;

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.UseIsOperatorInsteadOfAsOperator, node);
        }
    }
}
