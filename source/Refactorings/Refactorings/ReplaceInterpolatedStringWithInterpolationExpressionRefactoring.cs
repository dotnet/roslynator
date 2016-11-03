// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceInterpolatedStringWithInterpolationExpressionRefactoring
    {
        public static bool CanRefactor(InterpolatedStringExpressionSyntax interpolatedString)
        {
            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            if (contents.Count == 1)
            {
                InterpolatedStringContentSyntax content = contents[0];

                if (content.IsKind(SyntaxKind.Interpolation))
                {
                    var interpolation = (InterpolationSyntax)content;

                    if (interpolation?.IsMissing == false)
                        return true;
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var interpolation = (InterpolationSyntax)interpolatedString.Contents[0];

            ExpressionSyntax newNode = interpolation.Expression;

            newNode = newNode
                .PrependLeadingTrivia(interpolation.GetLeadingTrivia()
                    .Concat(interpolation.OpenBraceToken.TrailingTrivia))
                .AppendTrailingTrivia(interpolation.CloseBraceToken.LeadingTrivia
                    .Concat(interpolatedString.GetTrailingTrivia()));

            SyntaxNode newRoot = root.ReplaceNode(interpolatedString, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
