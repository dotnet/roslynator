// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceInterpolatedStringWithConcatenationRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            CancellationToken cancellationToken)
        {
            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            ExpressionSyntax newNode = AddExpression(
                ((InterpolationSyntax)contents[0]).Expression.Parenthesize(),
                ((InterpolationSyntax)contents[1]).Expression.Parenthesize());

            for (int i = 2; i < contents.Count; i++)
            {
                newNode = AddExpression(
                    newNode,
                    ((InterpolationSyntax)contents[i]).Expression.Parenthesize());
            }

            newNode = newNode
                .WithTriviaFrom(interpolatedString)
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(interpolatedString, newNode, cancellationToken);
        }
    }
}
