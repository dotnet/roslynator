// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Utilities;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeInterpolationIntoInterpolatedStringRefactoring
    {
        public static bool CanRefactor(InterpolationSyntax interpolation)
        {
            if (interpolation == null)
                throw new ArgumentNullException(nameof(interpolation));

            ExpressionSyntax expression = interpolation.Expression;

            if (expression?.IsKind(SyntaxKind.StringLiteralExpression) == true)
            {
                var interpolatedString = interpolation.Parent as InterpolatedStringExpressionSyntax;

                if (interpolatedString != null)
                {
                    var literalExpression = (LiteralExpressionSyntax)expression;

                    if (interpolatedString.IsVerbatim() == literalExpression.Token.IsVerbatimStringLiteral())
                        return true;
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InterpolationSyntax interpolation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (interpolation == null)
                throw new ArgumentNullException(nameof(interpolation));

            var interpolatedString = (InterpolatedStringExpressionSyntax)interpolation.Parent;

            string s = interpolatedString.ToString();

            var literalExpression = (LiteralExpressionSyntax)interpolation.Expression;

            s = s.Substring(0, interpolation.Span.Start - interpolatedString.Span.Start)
                + StringUtility.DoubleBraces(literalExpression.GetStringLiteralInnerText())
                + s.Substring(interpolation.Span.End - interpolatedString.Span.Start);

            var newInterpolatedString = (InterpolatedStringExpressionSyntax)SyntaxFactory.ParseExpression(s)
                .WithTriviaFrom(interpolatedString);

            return document.ReplaceNodeAsync(interpolatedString, newInterpolatedString, cancellationToken);
        }
    }
}
