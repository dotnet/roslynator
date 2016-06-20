// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class ConvertInterpolatedStringToStringLiteralRefactoring
    {
        public static bool CanRefactor(InterpolatedStringExpressionSyntax interpolatedString)
        {
            if (interpolatedString == null)
                throw new ArgumentNullException(nameof(interpolatedString));

            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            return contents.Count == 0
                || (contents.Count == 1 && contents[0].IsKind(SyntaxKind.InterpolatedStringText));
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (interpolatedString == null)
                throw new ArgumentNullException(nameof(interpolatedString));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var newNode = (LiteralExpressionSyntax)SyntaxFactory.ParseExpression(interpolatedString.ToString().Substring(1))
                .WithTriviaFrom(interpolatedString);

            SyntaxNode newRoot = oldRoot.ReplaceNode(interpolatedString, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
