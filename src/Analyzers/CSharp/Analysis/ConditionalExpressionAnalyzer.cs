// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConditionalExpressionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AvoidNestedConditionalOperators); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeConditionalExpression(f), SyntaxKind.ConditionalExpression);
        }

        private static void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
        {
            var conditionalExpression = (ConditionalExpressionSyntax)context.Node;

            if (!conditionalExpression.WalkUpParentheses().IsParentKind(SyntaxKind.ConditionalExpression))
            {
                if (conditionalExpression.WhenTrue.WalkDownParentheses().IsKind(SyntaxKind.ConditionalExpression)
                    || conditionalExpression.WhenFalse.WalkDownParentheses().IsKind(SyntaxKind.ConditionalExpression))
                {
                    DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.AvoidNestedConditionalOperators, conditionalExpression);
                }
            }
        }
    }
}
