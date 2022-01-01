// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class PlaceNewLineAfterOrBeforeConditionalOperatorAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.PlaceNewLineAfterOrBeforeConditionalOperator);
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

            ExpressionSyntax condition = conditionalExpression.Condition;

            if (condition.IsMissing)
                return;

            ExpressionSyntax whenTrue = conditionalExpression.WhenTrue;

            if (whenTrue.IsMissing)
                return;

            NewLinePosition newLinePosition = context.GetConditionalExpressionNewLinePosition();

            if (newLinePosition == NewLinePosition.None)
                return;

            if (SyntaxTriviaAnalysis.IsTokenFollowedWithNewLineAndNotPrecededWithNewLine(condition, conditionalExpression.QuestionToken, whenTrue))
            {
                if (newLinePosition == NewLinePosition.Before)
                {
                    ReportDiagnostic(conditionalExpression.QuestionToken, ImmutableDictionary<string, string>.Empty, "before");
                    return;
                }
            }
            else if (SyntaxTriviaAnalysis.IsTokenPrecededWithNewLineAndNotFollowedWithNewLine(condition, conditionalExpression.QuestionToken, whenTrue))
            {
                if (newLinePosition == NewLinePosition.After)
                {
                    ReportDiagnostic(conditionalExpression.QuestionToken, DiagnosticProperties.AnalyzerOption_Invert, "after");
                    return;
                }
            }

            ExpressionSyntax whenFalse = conditionalExpression.WhenFalse;

            if (!whenFalse.IsMissing)
            {
                if (SyntaxTriviaAnalysis.IsTokenFollowedWithNewLineAndNotPrecededWithNewLine(whenTrue, conditionalExpression.ColonToken, whenFalse))
                {
                    if (newLinePosition == NewLinePosition.Before)
                        ReportDiagnostic(conditionalExpression.ColonToken, ImmutableDictionary<string, string>.Empty, "before");
                }
                else if (SyntaxTriviaAnalysis.IsTokenPrecededWithNewLineAndNotFollowedWithNewLine(whenTrue, conditionalExpression.ColonToken, whenFalse))
                {
                    if (newLinePosition == NewLinePosition.After)
                        ReportDiagnostic(conditionalExpression.ColonToken, DiagnosticProperties.AnalyzerOption_Invert, "after");
                }
            }

            void ReportDiagnostic(
                SyntaxToken token,
                ImmutableDictionary<string, string> properties,
                string messageArg)
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.PlaceNewLineAfterOrBeforeConditionalOperator,
                    Location.Create(token.SyntaxTree, token.Span.WithLength(0)),
                    properties: properties,
                    messageArg);
            }
        }
    }
}
