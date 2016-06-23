// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class AddInterpolationRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, InterpolatedStringTextSyntax interpolatedStringText)
        {
            return !context.Span.IsEmpty
                && interpolatedStringText.Parent?.IsKind(SyntaxKind.InterpolatedStringExpression) == true;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringTextSyntax interpolatedStringText,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            var interpolatedString = (InterpolatedStringExpressionSyntax)interpolatedStringText.Parent;

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
