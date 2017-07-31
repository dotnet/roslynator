// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatConditionalExpressionRefactoring
    {
        public static void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
        {
            var conditionalExpression = (ConditionalExpressionSyntax)context.Node;

            if (!conditionalExpression.ContainsDiagnostics
                && !conditionalExpression.SpanContainsDirectives())
            {
                if (IsFixable(conditionalExpression.Condition, conditionalExpression.QuestionToken)
                    || IsFixable(conditionalExpression.WhenTrue, conditionalExpression.ColonToken))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.FormatConditionalExpression,
                        conditionalExpression);
                }
            }
        }

        private static bool IsFixable(ExpressionSyntax expression, SyntaxToken token)
        {
            SyntaxTriviaList expressionTrailing = expression.GetTrailingTrivia();

            if (expressionTrailing.IsEmptyOrWhitespace())
            {
                SyntaxTriviaList tokenLeading = token.LeadingTrivia;

                if (tokenLeading.IsEmptyOrWhitespace())
                {
                    SyntaxTriviaList tokenTrailing = token.TrailingTrivia;

                    int count = tokenTrailing.Count;

                    if (count == 1)
                    {
                        if (tokenTrailing[0].IsEndOfLineTrivia())
                            return true;
                    }
                    else if (count > 1)
                    {
                        for (int i = 0; i < count - 1; i++)
                        {
                            if (!tokenTrailing[i].IsWhitespaceTrivia())
                                return false;
                        }

                        if (tokenTrailing.Last().IsEndOfLineTrivia())
                            return true;
                    }
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = conditionalExpression.Condition;
            ExpressionSyntax whenTrue = conditionalExpression.WhenTrue;
            ExpressionSyntax whenFalse = conditionalExpression.WhenFalse;
            SyntaxToken questionToken = conditionalExpression.QuestionToken;
            SyntaxToken colonToken = conditionalExpression.ColonToken;

            var builder = new SyntaxNodeTextBuilder(conditionalExpression);

            builder.AppendLeadingTrivia();
            builder.AppendSpan(condition);

            Write(condition, whenTrue, questionToken, "? ", builder);

            Write(whenTrue, whenFalse, colonToken, ": ", builder);

            builder.AppendTrailingTrivia();

            ExpressionSyntax newNode = SyntaxFactory.ParseExpression(builder.ToString());

            return document.ReplaceNodeAsync(conditionalExpression, newNode, cancellationToken);
        }

        private static void Write(
            ExpressionSyntax expression,
            ExpressionSyntax nextExpression,
            SyntaxToken token,
            string newText,
            SyntaxNodeTextBuilder builder)
        {
            if (IsFixable(expression, token))
            {
                if (!expression.GetTrailingTrivia().IsEmptyOrWhitespace()
                    || !token.LeadingTrivia.IsEmptyOrWhitespace())
                {
                    builder.AppendTrailingTrivia(expression);
                    builder.AppendLeadingTrivia(token);
                }

                builder.AppendTrailingTrivia(token);
                builder.AppendLeadingTrivia(nextExpression);
                builder.Append(newText);
                builder.AppendSpan(nextExpression);
            }
            else
            {
                builder.AppendTrailingTrivia(expression);
                builder.AppendFullSpan(token);
                builder.AppendLeadingTriviaAndSpan(nextExpression);
            }
        }
    }
}
