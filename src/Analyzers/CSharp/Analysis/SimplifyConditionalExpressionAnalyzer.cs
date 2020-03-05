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
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.SimplifyConditionalExpression,
                    DiagnosticDescriptors.SimplifyConditionalExpression2);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeConditionalExpression, SyntaxKind.ConditionalExpression);
        }

        private static void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
        {
            var conditionalExpression = (ConditionalExpressionSyntax)context.Node;

            if (context.Node.ContainsDiagnostics)
                return;

            if (context.Node.SpanContainsDirectives())
                return;

            ConditionalExpressionInfo info = SyntaxInfo.ConditionalExpressionInfo(conditionalExpression);

            if (!info.Success)
                return;

            SyntaxKind trueKind = info.WhenTrue.Kind();
            SyntaxKind falseKind = info.WhenFalse.Kind();

            if (trueKind == SyntaxKind.TrueLiteralExpression)
            {
                // a ? true : false >>> a
                // a ? true : b >>> a || b
                if (falseKind == SyntaxKind.FalseLiteralExpression
                    || context.SemanticModel.GetTypeInfo(info.WhenFalse, context.CancellationToken).ConvertedType?.SpecialType == SpecialType.System_Boolean)
                {
                    Report(DiagnosticDescriptors.SimplifyConditionalExpression);
                }
            }
            else if (trueKind == SyntaxKind.FalseLiteralExpression)
            {
                /// a ? false : true >>> !a
                if (falseKind == SyntaxKind.TrueLiteralExpression)
                {
                    Report(DiagnosticDescriptors.SimplifyConditionalExpression);
                }
                /// a ? false : b >>> !a && b
                else if (context.SemanticModel.GetTypeInfo(info.WhenFalse, context.CancellationToken).ConvertedType?.SpecialType == SpecialType.System_Boolean)
                {
                    Report(DiagnosticDescriptors.SimplifyConditionalExpression2);
                }
            }
            else if (falseKind == SyntaxKind.TrueLiteralExpression)
            {
                // a ? b : true >>> !a || b
                if (context.SemanticModel.GetTypeInfo(info.WhenTrue, context.CancellationToken).ConvertedType?.SpecialType == SpecialType.System_Boolean)
                {
                    Report(DiagnosticDescriptors.SimplifyConditionalExpression2);
                }
            }
            else if (falseKind == SyntaxKind.FalseLiteralExpression)
            {
                // a ? b : false >>> a && b
                if (context.SemanticModel.GetTypeInfo(info.WhenTrue, context.CancellationToken).ConvertedType?.SpecialType == SpecialType.System_Boolean)
                {
                    Report(DiagnosticDescriptors.SimplifyConditionalExpression);
                }
            }

            void Report(DiagnosticDescriptor descriptor)
            {
                DiagnosticHelpers.ReportDiagnosticIfNotSuppressed(context, descriptor, conditionalExpression);
            }
        }
    }
}
