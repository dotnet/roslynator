// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class CommentTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SyntaxTrivia trivia)
        {
            if (context.Root.IsKind(SyntaxKind.CompilationUnit)
                && trivia.IsCommentTrivia())
            {
                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RemoveComment))
                {
                    context.RegisterRefactoring(
                        "Remove comment",
                        cancellationToken => RemoveCommentRefactoring.RemoveCommentAsync(context.Document, trivia, cancellationToken));
                }

                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllComments))
                {
                    context.RegisterRefactoring(
                        "Remove all comments",
                        cancellationToken => RemoveCommentAsync(context.Document, CommentRemoveOptions.All, cancellationToken: cancellationToken));
                }

                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllCommentsExceptXmlComments)
                    && trivia.IsKind(SyntaxKind.SingleLineCommentTrivia, SyntaxKind.MultiLineCommentTrivia))
                {
                    context.RegisterRefactoring(
                        "Remove all comments (except xml comments)",
                        cancellationToken => RemoveCommentAsync(context.Document, CommentRemoveOptions.AllExceptDocumentation, cancellationToken: cancellationToken));
                }

                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllXmlComments)
                    && trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
                {
                    context.RegisterRefactoring(
                        "Remove all xml comments",
                        cancellationToken => RemoveCommentAsync(context.Document, CommentRemoveOptions.Documentation, cancellationToken: cancellationToken));
                }
            }
        }

        private static async Task<Document> RemoveCommentAsync(
            Document document,
            CommentRemoveOptions removeOptions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            root = SyntaxRemover.RemoveComment(root, removeOptions)
                .WithFormatterAnnotation();

            return document.WithSyntaxRoot(root);
        }
    }
}