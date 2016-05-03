// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(InterpolatedStringCodeRefactoringProvider))]
    public class InterpolatedStringCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            InterpolatedStringExpressionSyntax interpolatedString = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InterpolatedStringExpressionSyntax>();

            if (interpolatedString == null)
                return;

            if (CanConvertToInterpolatedString(interpolatedString))
            {
                context.RegisterRefactoring("Convert to string literal",
                    cancellationToken =>
                    {
                        return ConvertToInterpolatedStringAsync(
                            context.Document,
                            interpolatedString,
                            cancellationToken);
                    });
            }
        }

        private static bool CanConvertToInterpolatedString(InterpolatedStringExpressionSyntax interpolatedString)
        {
            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            return contents.Count == 0
                || (contents.Count == 1 && contents[0].IsKind(SyntaxKind.InterpolatedStringText));
        }

        private static async Task<Document> ConvertToInterpolatedStringAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var newNode = (LiteralExpressionSyntax)SyntaxFactory.ParseExpression(interpolatedString.ToString().Substring(1))
                .WithTriviaFrom(interpolatedString);

            SyntaxNode newRoot = oldRoot.ReplaceNode(interpolatedString, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
