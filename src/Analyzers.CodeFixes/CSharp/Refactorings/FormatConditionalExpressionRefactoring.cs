// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatConditionalExpressionRefactoring
    {
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

            StringBuilder sb = StringBuilderCache.GetInstance();

            var builder = new SyntaxNodeTextBuilder(conditionalExpression, sb);

            builder.AppendLeadingTrivia();
            builder.AppendSpan(condition);

            Write(condition, whenTrue, questionToken, "? ", builder);

            Write(whenTrue, whenFalse, colonToken, ": ", builder);

            builder.AppendTrailingTrivia();

            ExpressionSyntax newNode = SyntaxFactory.ParseExpression(StringBuilderCache.GetStringAndFree(sb));

            return document.ReplaceNodeAsync(conditionalExpression, newNode, cancellationToken);
        }

        private static void Write(
            ExpressionSyntax expression,
            ExpressionSyntax nextExpression,
            SyntaxToken token,
            string newText,
            SyntaxNodeTextBuilder builder)
        {
            if (FormatConditionalExpressionAnalyzer.IsFixable(expression, token))
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
