// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AddOrRemoveParenthesesFromConditionInConditionalExpressionAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.AddOrRemoveParenthesesFromConditionInConditionalOperator);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeConditionalExpression(f), SyntaxKind.ConditionalExpression);
        }

        private static void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
        {
            var conditionalExpression = (ConditionalExpressionSyntax)context.Node;

            if (conditionalExpression.ContainsDiagnostics)
                return;

            ExpressionSyntax condition = conditionalExpression.Condition;

            if (condition == null)
                return;

            ConditionalExpressionParenthesesStyle style = context.GetConditionalExpressionParenthesesStyle();

            if (style == ConditionalExpressionParenthesesStyle.None)
                return;

            SyntaxKind kind = condition.Kind();

            if (kind == SyntaxKind.ParenthesizedExpression)
            {
                var parenthesizedExpression = (ParenthesizedExpressionSyntax)condition;

                ExpressionSyntax expression = parenthesizedExpression.Expression;

                if (!expression.IsMissing)
                {
                    if (style == ConditionalExpressionParenthesesStyle.Omit
                        || (style == ConditionalExpressionParenthesesStyle.OmitWhenConditionIsSingleToken
                            && CSharpFacts.IsSingleTokenExpression(expression.Kind())))
                    {
                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.AddOrRemoveParenthesesFromConditionInConditionalOperator, condition, "Remove", "from");
                    }
                }
            }
            else if (style == ConditionalExpressionParenthesesStyle.Include
                || (style == ConditionalExpressionParenthesesStyle.OmitWhenConditionIsSingleToken
                    && !CSharpFacts.IsSingleTokenExpression(kind)))
            {
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.AddOrRemoveParenthesesFromConditionInConditionalOperator, condition, "Add", "to");
            }
        }
    }
}
