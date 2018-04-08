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
    public class JoinStringExpressionsAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.JoinStringExpressions); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeAddExpression, SyntaxKind.AddExpression);
        }

        public static void AnalyzeAddExpression(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

            if (node.ContainsDiagnostics)
                return;

            if (node.SpanContainsDirectives())
                return;

            var addExpression = (BinaryExpressionSyntax)node;

            StringConcatenationExpressionInfo concatenationInfo = SyntaxInfo.StringConcatenationExpressionInfo(addExpression, context.SemanticModel, context.CancellationToken);

            if (!concatenationInfo.Success)
                return;

            StringConcatenationAnalysis analysis = concatenationInfo.Analyze();

            if (!analysis.ContainsUnspecifiedExpression
                && (analysis.ContainsStringLiteral ^ analysis.ContainsInterpolatedString)
                && (analysis.ContainsNonVerbatimExpression ^ analysis.ContainsVerbatimExpression)
                && (analysis.ContainsVerbatimExpression || addExpression.IsSingleLine(includeExteriorTrivia: false, cancellationToken: context.CancellationToken)))
            {
                context.ReportDiagnostic(DiagnosticDescriptors.JoinStringExpressions, addExpression);
            }
        }
    }
}
