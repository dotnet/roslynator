// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
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
            bool addNameOf = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string s = interpolatedString.ToString();

            int startIndex = span.Start - interpolatedString.SpanStart;

            var sb = new StringBuilder();

            sb.Append(s, 0, startIndex);
            sb.Append('{');

            if (addNameOf)
            {
                string identifier = StringLiteralParser.Parse(
                    s,
                    startIndex,
                    span.Length,
                    isVerbatim: interpolatedString.IsVerbatim(),
                    isInterpolatedText: true);

                sb.Append(CSharpFactory.NameOfExpression(identifier));
            }

            int closeBracePosition = sb.Length;

            sb.Append('}');

            startIndex += span.Length;
            sb.Append(s, startIndex, s.Length - startIndex);

            ExpressionSyntax newNode = ParseExpression(sb.ToString()).WithTriviaFrom(interpolatedString);

            SyntaxToken closeBrace = newNode.FindToken(closeBracePosition);

            newNode = newNode.ReplaceToken(closeBrace, closeBrace.WithNavigationAnnotation());

            return document.ReplaceNodeAsync(interpolatedString, newNode, cancellationToken);
        }
    }
}
