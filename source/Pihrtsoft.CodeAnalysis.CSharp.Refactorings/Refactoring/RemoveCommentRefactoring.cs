// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RemoveCommentRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SyntaxTrivia trivia)
        {
            if (CommentRefactoring.CanRemove(trivia))
            {
                context.RegisterRefactoring(
                    "Remove comment",
                    cancellationToken => CommentRefactoring.RemoveCommentAsync(context.Document, trivia, cancellationToken));

                context.RegisterRefactoring(
                    "Remove all comments",
                    cancellationToken => CommentRefactoring.RemoveAllCommentsAsync(context.Document, cancellationToken));

                if (trivia.IsAnyKind(SyntaxKind.SingleLineCommentTrivia, SyntaxKind.MultiLineCommentTrivia))
                {
                    context.RegisterRefactoring(
                        "Remove all comments (except xml comments)",
                        cancellationToken => CommentRefactoring.RemoveAllCommentsExceptXmlCommentsAsync(context.Document, cancellationToken));
                }
            }
        }
    }
}