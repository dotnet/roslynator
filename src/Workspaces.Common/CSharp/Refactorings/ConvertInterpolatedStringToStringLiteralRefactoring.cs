// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertInterpolatedStringToStringLiteralRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            CancellationToken cancellationToken = default)
        {
            string s = interpolatedString.ToString();

            s = s.Remove(interpolatedString.StringStartToken.Text.IndexOf('$'), 1);

            s = StringUtility.ReplaceDoubleBracesWithSingleBrace(s);

            var newNode = (LiteralExpressionSyntax)SyntaxFactory.ParseExpression(s)
                .WithTriviaFrom(interpolatedString);

            return document.ReplaceNodeAsync(interpolatedString, newNode, cancellationToken);
        }
    }
}
