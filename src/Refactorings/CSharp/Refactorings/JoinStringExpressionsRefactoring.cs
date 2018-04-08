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
            StringConcatenationAnalysis analysis = concatenationInfo.Analyze();

            if (analysis.ContainsNonStringLiteral)
            {
                if (analysis.ContainsStringLiteral || analysis.ContainsInterpolatedString)
                {
                    context.RegisterRefactoring(
                        "Join string expressions",
                        cancellationToken => ToInterpolatedStringAsync(context.Document, concatenationInfo, cancellationToken));
                }
            }
            else if (analysis.ContainsStringLiteral)
            {
                context.RegisterRefactoring(
                    "Join string literals",
                    cancellationToken => ToStringLiteralAsync(context.Document, concatenationInfo, multiline: false, cancellationToken: cancellationToken));

                if (concatenationInfo.BinaryExpression
                    .DescendantTrivia(concatenationInfo.Span ?? concatenationInfo.BinaryExpression.Span)
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
            InterpolatedStringExpressionSyntax newExpression = concatenationInfo.ToInterpolatedStringExpression();

            return RefactorAsync(document, concatenationInfo, newExpression, cancellationToken);
        }

        public static Task<Document> ToStringLiteralAsync(
            Document document,
            StringConcatenationExpressionInfo concatenationInfo,
            bool multiline,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax newExpression = (multiline)
                ? concatenationInfo.ToMultiLineStringLiteralExpression()
                : concatenationInfo.ToStringLiteralExpression();

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

                int start = concatenationInfo.BinaryExpression.SpanStart;

                string s = concatenationInfo.BinaryExpression.ToString();

                s = s.Remove(span.Start - start)
                    + expression
                    + s.Substring(span.End - start);

                expression = SyntaxFactory.ParseExpression(s);
            }

            expression = expression
                .WithTriviaFrom(concatenationInfo.BinaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(concatenationInfo.BinaryExpression, expression, cancellationToken);
        }
    }
}
