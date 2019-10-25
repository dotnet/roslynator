// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AddNewLineBeforeConditionalOperatorInsteadOfAfterItAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddNewLineBeforeConditionalOperatorInsteadOfAfterIt); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeConditionalExpression, SyntaxKind.ConditionalExpression);
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

            if (SyntaxTriviaAnalysis.IsTokenFollowedWithNewLineAndNotPrecededWithNewLine(condition, conditionalExpression.QuestionToken, whenTrue))
            {
                ReportDiagnostic(context, conditionalExpression.QuestionToken);
            }
            else
            {
                ExpressionSyntax whenFalse = conditionalExpression.WhenFalse;

                if (!whenFalse.IsMissing
                    && SyntaxTriviaAnalysis.IsTokenFollowedWithNewLineAndNotPrecededWithNewLine(whenTrue, conditionalExpression.ColonToken, whenFalse))
                {
                    ReportDiagnostic(context, conditionalExpression.ColonToken);
                }
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxToken token)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.AddNewLineBeforeConditionalOperatorInsteadOfAfterIt,
                Location.Create(token.SyntaxTree, token.Span.WithLength(0)));
        }
    }
}
