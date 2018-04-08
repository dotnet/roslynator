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
    public class SimplifyConditionalExpressionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.SimplifyConditionalExpression); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeConditionalExpression, SyntaxKind.ConditionalExpression);
        }

        public static void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
        {
            var conditionalExpression = (ConditionalExpressionSyntax)context.Node;

            if (context.Node.ContainsDiagnostics)
                return;

            if (context.Node.SpanContainsDirectives())
                return;

            ConditionalExpressionInfo info = SyntaxInfo.ConditionalExpressionInfo(conditionalExpression);

            if (!info.Success)
                return;

            switch (info.WhenTrue.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                    {
                        if (info.WhenFalse.Kind() == SyntaxKind.FalseLiteralExpression)
                            context.ReportDiagnostic(DiagnosticDescriptors.SimplifyConditionalExpression, conditionalExpression);

                        break;
                    }
                case SyntaxKind.FalseLiteralExpression:
                    {
                        if (info.WhenFalse.Kind() == SyntaxKind.TrueLiteralExpression)
                            context.ReportDiagnostic(DiagnosticDescriptors.SimplifyConditionalExpression, conditionalExpression);

                        break;
                    }
            }
        }
    }
}
