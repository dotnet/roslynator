// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class JoinStringExpressionsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, StringConcatenationExpressionInfo concatenationInfo)
        {
            if (concatenationInfo.ContainsNonLiteralExpression)
            {
                if (concatenationInfo.ContainsLiteralExpression || concatenationInfo.ContainsInterpolatedStringExpression)
                {
                    context.RegisterRefactoring(
                        "Join string expressions",
                        cancellationToken => ToInterpolatedStringAsync(context.Document, concatenationInfo, cancellationToken));
                }
            }
            else if (concatenationInfo.ContainsLiteralExpression)
            {
                context.RegisterRefactoring(
                    "Join string literals",
                    cancellationToken => ToStringLiteralAsync(context.Document, concatenationInfo, multiline: false, cancellationToken: cancellationToken));

                if (concatenationInfo.OriginalExpression
                        .DescendantTrivia(concatenationInfo.Span ?? concatenationInfo.OriginalExpression.Span)
                        .Any(f => f.IsEndOfLineTrivia()))
                {
                    context.RegisterRefactoring(
                        "Join string literals into multiline string literal",
                        cancellationToken => ToStringLiteralAsync(context.Document, concatenationInfo, multiline: true, cancellationToken: cancellationToken));
                }
            }
        }

        private static Task<Document> ToInterpolatedStringAsync(
            Document document,
            StringConcatenationExpressionInfo concatenationInfo,
            CancellationToken cancellationToken)
        {
            InterpolatedStringExpressionSyntax newExpression = concatenationInfo.ToInterpolatedString();

            return RefactorAsync(document, concatenationInfo, newExpression, cancellationToken);
        }

        public static Task<Document> ToStringLiteralAsync(
            Document document,
            StringConcatenationExpressionInfo concatenationInfo,
            bool multiline,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax newExpression = (multiline)
                ? concatenationInfo.ToMultilineStringLiteral()
                : concatenationInfo.ToStringLiteral();

            return RefactorAsync(document, concatenationInfo, newExpression, cancellationToken);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            StringConcatenationExpressionInfo concatenationInfo,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            if (concatenationInfo.Span.HasValue)
            {
                TextSpan span = concatenationInfo.Span.Value;

                int start = concatenationInfo.OriginalExpression.SpanStart;

                string s = concatenationInfo.OriginalExpression.ToString();

                s = s.Remove(span.Start - start)
                    + expression
                    + s.Substring(span.End - start);

                expression = SyntaxFactory.ParseExpression(s);
            }

            expression = expression
                .WithTriviaFrom(concatenationInfo.OriginalExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(concatenationInfo.OriginalExpression, expression, cancellationToken);
        }
    }
}
