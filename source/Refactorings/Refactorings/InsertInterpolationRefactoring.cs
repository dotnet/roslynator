// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InsertInterpolationRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, InterpolatedStringExpressionSyntax interpolatedString)
        {
            int i = 0;
            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            foreach (InterpolatedStringContentSyntax content in contents)
            {
                SyntaxKind kind = content.Kind();
                TextSpan span = content.Span;

                if (kind == SyntaxKind.InterpolatedStringText)
                {
                    if (span.End == context.Span.End)
                        return true;
                }
                else if (kind == SyntaxKind.Interpolation)
                {
                    if (span.Start == context.Span.End)
                        return true;

                    if (span.End == context.Span.Start
                        && (i == contents.Count - 1 || !contents[i + 1].IsKind(SyntaxKind.InterpolatedStringText)))
                    {
                        return true;
                    }
                }

                i++;
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            string s = interpolatedString.ToString();

            int startIndex = span.Start - interpolatedString.SpanStart;

            s = s.Substring(0, startIndex) +
                "{" +
                s.Substring(startIndex, span.Length) +
                "}" +
                s.Substring(startIndex + span.Length);

            var newNode = (InterpolatedStringExpressionSyntax)ParseExpression(s)
                .WithTriviaFrom(interpolatedString);

            root = root.ReplaceNode(interpolatedString, newNode);

            return document.WithSyntaxRoot(root);
        }
    }
}
