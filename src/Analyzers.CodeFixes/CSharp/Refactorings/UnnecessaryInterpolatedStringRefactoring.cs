// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UnnecessaryInterpolatedStringRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            CancellationToken cancellationToken)
        {
            var interpolation = (InterpolationSyntax)interpolatedString.Contents[0];

            ExpressionSyntax newNode = interpolation.Expression
                .WithTriviaFrom(interpolatedString)
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(interpolatedString, newNode, cancellationToken);
        }
    }
}
