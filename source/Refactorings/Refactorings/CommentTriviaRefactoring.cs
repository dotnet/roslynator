// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CommentTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SyntaxTrivia trivia)
        {
            SyntaxKind kind = trivia.Kind();

            if (context.IsRootCompilationUnit
                && trivia.FullSpan.Contains(context.Span)
                && IsComment(kind))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveComment))
                {
                    context.RegisterRefactoring(
                        "Remove comment",
                        cancellationToken => context.Document.RemoveCommentAsync(trivia, cancellationToken));
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllComments))
                {
                    context.RegisterRefactoring(
                        "Remove all comments",
                        cancellationToken => context.Document.RemoveCommentsAsync(CommentRemoveOptions.All, cancellationToken));
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllCommentsExceptDocumentationComments)
                    && (kind == SyntaxKind.SingleLineCommentTrivia || kind == SyntaxKind.MultiLineCommentTrivia))
                {
                    context.RegisterRefactoring(
                        "Remove all comments (except documentation comments)",
                        cancellationToken => context.Document.RemoveCommentsAsync(CommentRemoveOptions.AllExceptDocumentation, cancellationToken));
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllDocumentationComments)
                    && IsDocumentationComment(kind))
                {
                    context.RegisterRefactoring(
                        "Remove all documentation comments",
                        cancellationToken => context.Document.RemoveCommentsAsync(CommentRemoveOptions.Documentation, cancellationToken));
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
                        cancellationToken => context.Document.RemoveCommentsAsync(CommentRemoveOptions.All, context.Span, cancellationToken));
                }

                if (fComment
                    && fDocComment
                    && context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllCommentsExceptDocumentationComments))
                {
                    context.RegisterRefactoring(
                        "Remove comments (except documentation comments)",
                        cancellationToken => context.Document.RemoveCommentsAsync(CommentRemoveOptions.AllExceptDocumentation, context.Span, cancellationToken));
                }

                if (fDocComment
                    && context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllDocumentationComments))
                {
                    context.RegisterRefactoring(
                        "Remove documentation comments",
                        c => context.Document.RemoveCommentsAsync(CommentRemoveOptions.Documentation, context.Span, c));
                }
            }
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