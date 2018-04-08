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
    public class ParenthesizeConditionInConditionalExpressionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.ParenthesizeConditionInConditionalExpression); }
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

            if (conditionalExpression.ContainsDiagnostics)
                return;

            ConditionalExpressionInfo info = SyntaxInfo.ConditionalExpressionInfo(conditionalExpression, walkDownParentheses: false);

            if (!info.Success)
                return;

            if (info.Condition.Kind() == SyntaxKind.ParenthesizedExpression)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.ParenthesizeConditionInConditionalExpression, info.Condition);
        }
    }
}
