// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ParenthesizeConditionOfConditionalExpressionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.ParenthesizeConditionOfConditionalExpression); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeConditionalExpression, SyntaxKind.ConditionalExpression);
        }

        private static void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
        {
            var conditionalExpression = (ConditionalExpressionSyntax)context.Node;

            if (conditionalExpression.ContainsDiagnostics)
                return;

            ExpressionSyntax condition = conditionalExpression.Condition;

            if (condition == null)
                return;

            SyntaxKind kind = condition.Kind();

            if (kind == SyntaxKind.ParenthesizedExpression)
            {
                if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.RemoveParenthesesFromConditionOfConditionalExpressionWhenExpressionIsSingleToken))
                {
                    var parenthesizedExpression = (ParenthesizedExpressionSyntax)condition;

                    ExpressionSyntax expression = parenthesizedExpression.Expression;

                    if (!expression.IsMissing
                        && CSharpFacts.IsSingleTokenExpression(expression.Kind()))
                    {
                        ReportDiagnostic("Remove parentheses from");
                    }
                }
            }
            else if (!CSharpFacts.IsSingleTokenExpression(kind)
                || context.IsAnalyzerSuppressed(DiagnosticDescriptors.RemoveParenthesesFromConditionOfConditionalExpressionWhenExpressionIsSingleToken))
            {
                ReportDiagnostic("Parenthesize");
            }

            void ReportDiagnostic(params string[] messageArgs)
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticDescriptors.ParenthesizeConditionOfConditionalExpression,
                    condition,
                    messageArgs: messageArgs);
            }
        }
    }
}
