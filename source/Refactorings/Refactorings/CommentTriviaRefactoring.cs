// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CommentTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SyntaxTrivia trivia)
        {
            SyntaxKind kind = trivia.Kind();

            if (context.Root.IsKind(SyntaxKind.CompilationUnit)
                && trivia.FullSpan.Contains(context.Span)
                && IsComment(kind))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveComment))
                {
                    context.RegisterRefactoring(
                        "Remove comment",
                        c => RemoveCommentRefactoring.RemoveCommentAsync(context.Document, trivia, c));
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllComments))
                {
                    context.RegisterRefactoring(
                        "Remove all comments",
                        c => RemoveCommentAsync(context.Document, CommentRemoveOptions.All, c));
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllCommentsExceptDocumentationComments)
                    && (kind == SyntaxKind.SingleLineCommentTrivia || kind == SyntaxKind.MultiLineCommentTrivia))
                {
                    context.RegisterRefactoring(
                        "Remove all comments (except documentation comments)",
                        c => RemoveCommentAsync(context.Document, CommentRemoveOptions.AllExceptDocumentation, c));
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllDocumentationComments)
                    && IsDocumentationComment(kind))
                {
                    context.RegisterRefactoring(
                        "Remove all documentation comments",
                        c => RemoveCommentAsync(context.Document, CommentRemoveOptions.Documentation, c));
                }
            }
        }

        public static void ComputeRefactorings(RefactoringContext context, SyntaxNode node)
        {
            if (context.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.RemoveAllComments,
                RefactoringIdentifiers.RemoveAllCommentsExceptDocumentationComments,
                RefactoringIdentifiers.RemoveAllDocumentationComments))
            {
                bool fComment = false;
                bool fDocComment = false;

                foreach (SyntaxTrivia trivia in node.DescendantTrivia(context.Span, descendIntoTrivia: true))
                {
                    if (fComment && fDocComment)
                        break;

                    if (context.Span.Contains(trivia.Span))
                    {
                        switch (trivia.Kind())
                        {
                            case SyntaxKind.SingleLineCommentTrivia:
                            case SyntaxKind.MultiLineCommentTrivia:
                                {
                                    fComment = true;
                                    break;
                                }
                            case SyntaxKind.SingleLineDocumentationCommentTrivia:
                            case SyntaxKind.MultiLineDocumentationCommentTrivia:
                                {
                                    fDocComment = true;
                                    break;
                                }
                        }
                    }
                }

                if (fComment
                    && context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllComments))
                {
                    context.RegisterRefactoring(
                        "Remove comments",
                        c => RemoveCommentAsync(context.Document, CommentRemoveOptions.All, context.Span, c));
                }

                if (fComment
                    && fDocComment
                    && context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllCommentsExceptDocumentationComments))
                {
                    context.RegisterRefactoring(
                        "Remove comments (except documentation comments)",
                        c => RemoveCommentAsync(context.Document, CommentRemoveOptions.AllExceptDocumentation, context.Span, c));
                }

                if (fDocComment
                    && context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllDocumentationComments))
                {
                    context.RegisterRefactoring(
                        "Remove documentation comments",
                        c => RemoveCommentAsync(context.Document, CommentRemoveOptions.Documentation, context.Span, c));
                }
            }
        }

        private static async Task<Document> RemoveCommentAsync(
            Document document,
            CommentRemoveOptions removeOptions,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = SyntaxRemover.RemoveComment(root, removeOptions, span)
                .WithFormatterAnnotation();

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> RemoveCommentAsync(
            Document document,
            CommentRemoveOptions removeOptions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = SyntaxRemover.RemoveComment(root, removeOptions)
                .WithFormatterAnnotation();

            return document.WithSyntaxRoot(newRoot);
        }

        private static bool IsDocumentationComment(SyntaxKind kind)
        {
            return kind == SyntaxKind.SingleLineDocumentationCommentTrivia
                || kind == SyntaxKind.MultiLineDocumentationCommentTrivia;
        }

        public static bool IsComment(SyntaxKind kind)
        {
            return kind == SyntaxKind.SingleLineCommentTrivia
                || kind == SyntaxKind.SingleLineDocumentationCommentTrivia
                || kind == SyntaxKind.MultiLineCommentTrivia
                || kind == SyntaxKind.MultiLineDocumentationCommentTrivia;
        }
    }
}