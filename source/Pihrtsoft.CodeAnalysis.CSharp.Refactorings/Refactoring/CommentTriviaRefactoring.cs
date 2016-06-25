// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class CommentTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SyntaxTrivia trivia)
        {
            if (context.Root.IsKind(SyntaxKind.CompilationUnit)
                && trivia.IsCommentTrivia())
            {
                context.RegisterRefactoring(
                        "Remove comment",
                        cancellationToken => RemoveCommentRefactoring.RemoveCommentAsync(context.Document, trivia, cancellationToken));

                context.RegisterRefactoring(
                    "Remove all comments",
                    cancellationToken => RemoveAllCommentsRefactoring.RefactorAsync(context.Document, keepXmlComment: false, cancellationToken: cancellationToken));

                if (trivia.IsAnyKind(SyntaxKind.SingleLineCommentTrivia, SyntaxKind.MultiLineCommentTrivia))
                {
                    context.RegisterRefactoring(
                        "Remove all comments (except xml comments)",
                        cancellationToken => RemoveAllCommentsRefactoring.RefactorAsync(context.Document, keepXmlComment: true, cancellationToken: cancellationToken));
                }
            }
        }
    }
}