// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class JoinStringExpressionsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, StringConcatenationExpression concatenation)
        {
            if (concatenation.ContainsNonLiteralExpression)
            {
                if (concatenation.ContainsLiteralExpression || concatenation.ContainsInterpolatedStringExpression)
                {
                    context.RegisterRefactoring(
                        "Join string expressions",
                        cancellationToken => ToInterpolatedStringAsync(context.Document, concatenation, cancellationToken));
                }
            }
            else if (concatenation.ContainsLiteralExpression)
            {
                context.RegisterRefactoring(
                    "Join string literals",
                    cancellationToken => ToStringLiteralAsync(context.Document, concatenation, multiline: false, cancellationToken: cancellationToken));

                if (concatenation.OriginalExpression
                        .DescendantTrivia(concatenation.Span ?? concatenation.OriginalExpression.Span)
                        .Any(f => f.IsKind(SyntaxKind.EndOfLineTrivia)))
                {
                    context.RegisterRefactoring(
                        "Join string literals into multiline string literal",
                        cancellationToken => ToStringLiteralAsync(context.Document, concatenation, multiline: true, cancellationToken: cancellationToken));
                }
            }
        }

        private static Task<Document> ToInterpolatedStringAsync(
            Document document,
            StringConcatenationExpression concatenation,
            CancellationToken cancellationToken)
        {
            InterpolatedStringExpressionSyntax newExpression = concatenation.ToInterpolatedString();

            return RefactorAsync(document, concatenation, newExpression, cancellationToken);
        }

        public static Task<Document> ToStringLiteralAsync(
            Document document,
            StringConcatenationExpression concatenation,
            bool multiline,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax newExpression = (multiline)
                ? concatenation.ToMultilineStringLiteral()
                : concatenation.ToStringLiteral();

            return RefactorAsync(document, concatenation, newExpression, cancellationToken);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            StringConcatenationExpression concatenation,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            if (concatenation.Span.HasValue)
            {
                TextSpan span = concatenation.Span.Value;

                int start = concatenation.OriginalExpression.SpanStart;

                string s = concatenation.OriginalExpression.ToString();

                s = s.Remove(span.Start - start)
                    + expression
                    + s.Substring(span.End - start);

                expression = SyntaxFactory.ParseExpression(s);
            }

            expression = expression
                .WithTriviaFrom(concatenation.OriginalExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(concatenation.OriginalExpression, expression, cancellationToken);
        }
    }
}
