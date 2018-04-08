// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UnnecessaryInterpolationRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InterpolationSyntax interpolation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var interpolatedString = (InterpolatedStringExpressionSyntax)interpolation.Parent;

            string s = interpolatedString.ToString();

            s = s.Substring(0, interpolation.SpanStart - interpolatedString.SpanStart)
                + StringUtility.DoubleBraces(SyntaxInfo.StringLiteralExpressionInfo(interpolation.Expression).InnerText)
                + s.Substring(interpolation.Span.End - interpolatedString.SpanStart);

            var newInterpolatedString = (InterpolatedStringExpressionSyntax)SyntaxFactory.ParseExpression(s)
                .WithTriviaFrom(interpolatedString);

            return document.ReplaceNodeAsync(interpolatedString, newInterpolatedString, cancellationToken);
        }
    }
}
