// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            Document document = context.Document;
            TextSpan span = context.Span;

            if (context.IsRootCompilationUnit
                && trivia.FullSpan.Contains(span)
                && CSharpFacts.IsCommentTrivia(kind)
                && context.IsRefactoringEnabled(RefactoringDescriptors.RemoveComment))
            {
                context.RegisterRefactoring(
                    "Remove comment",
                    ct =>
                    {
                        SyntaxToken newToken = RemoveCommentHelper.GetReplacementToken(trivia).WithFormatterAnnotation();

                        return document.ReplaceTokenAsync(trivia.Token, newToken, ct);
                    },
                    RefactoringDescriptors.RemoveComment);
            }
        }

        public static void ComputeRefactorings(RefactoringContext context, SyntaxNode node)
        {
            TextSpan span = context.Span;

            if (context.IsAnyRefactoringEnabled(
                RefactoringDescriptors.RemoveAllComments,
                RefactoringDescriptors.RemoveAllCommentsExceptDocumentationComments,
                RefactoringDescriptors.RemoveAllDocumentationComments))
            {
                var fComment = false;
                var fDocComment = false;

                foreach (SyntaxTrivia trivia in node.DescendantTrivia(span, descendIntoTrivia: true))
                {
                    if (fComment && fDocComment)
                        break;

                    if (span.Contains(trivia.Span))
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

                Document document = context.Document;

                if ((fComment || fDocComment)
                    && context.IsRefactoringEnabled(RefactoringDescriptors.RemoveAllComments))
                {
                    context.RegisterRefactoring(
                        "Remove comments",
                        ct => document.RemoveCommentsAsync(span, CommentFilter.All, ct),
                        RefactoringDescriptors.RemoveAllComments);
                }

                if (fComment
                    && fDocComment
                    && context.IsRefactoringEnabled(RefactoringDescriptors.RemoveAllCommentsExceptDocumentationComments))
                {
                    context.RegisterRefactoring(
                        "Remove comments (except documentation comments)",
                        ct => document.RemoveCommentsAsync(span, CommentFilter.NonDocumentation, ct),
                        RefactoringDescriptors.RemoveAllCommentsExceptDocumentationComments);
                }

                if (fDocComment
                    && context.IsRefactoringEnabled(RefactoringDescriptors.RemoveAllDocumentationComments))
                {
                    context.RegisterRefactoring(
                        "Remove documentation comments",
                        ct => document.RemoveCommentsAsync(span, CommentFilter.Documentation, ct),
                        RefactoringDescriptors.RemoveAllDocumentationComments);
                }
            }
        }
    }
}
