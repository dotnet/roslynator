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
            TextSpan span = context.Span;

            int i = 0;
            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            foreach (InterpolatedStringContentSyntax content in contents)
            {
                SyntaxKind kind = content.Kind();
                TextSpan contentSpan = content.Span;

                if (kind == SyntaxKind.InterpolatedStringText)
                {
                    if (contentSpan.End == span.End)
                        return true;
                }
                else if (kind == SyntaxKind.Interpolation)
                {
                    if (contentSpan.Start == span.End)
                        return true;

                    if (contentSpan.End == span.Start
                        && (i == contents.Count - 1 || !contents[i + 1].IsKind(SyntaxKind.InterpolatedStringText)))
                    {
                        return true;
                    }
                }

                i++;
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string s = interpolatedString.ToString();

            int startIndex = span.Start - interpolatedString.SpanStart;

            s = s.Substring(0, startIndex) +
                "{" +
                s.Substring(startIndex, span.Length) +
                "}" +
                s.Substring(startIndex + span.Length);

            var newNode = (InterpolatedStringExpressionSyntax)ParseExpression(s)
                .WithTriviaFrom(interpolatedString);

            return document.ReplaceNodeAsync(interpolatedString, newNode, cancellationToken);
        }
    }
}
