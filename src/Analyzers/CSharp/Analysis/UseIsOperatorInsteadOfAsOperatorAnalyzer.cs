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
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeEqualsExpression, SyntaxKind.EqualsExpression);
            context.RegisterSyntaxNodeAction(AnalyzeNotEqualsExpression, SyntaxKind.NotEqualsExpression);
            context.RegisterSyntaxNodeAction(AnalyzeIsPatternExpression, SyntaxKind.IsPatternExpression);
        }

        public static void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            Analyze(context, context.Node);
        }

        public static void AnalyzeNotEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            Analyze(context, context.Node);
        }

        public static void AnalyzeIsPatternExpression(SyntaxNodeAnalysisContext context)
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

            context.ReportDiagnostic(DiagnosticDescriptors.UseIsOperatorInsteadOfAsOperator, node);
        }
    }
}
