// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Pihrtsoft.CodeAnalysis;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(SyntaxTriviaCodeRefactoringProvider))]
    public class SyntaxTriviaCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            SyntaxTrivia trivia = root.FindTrivia(context.Span.Start, findInsideTrivia: true);

            if (CommentRefactoring.CanRemove(trivia))
            {
                context.RegisterRefactoring(
                    "Remove comment",
                    cancellationToken => CommentRefactoring.RemoveCommentAsync(context.Document, trivia, cancellationToken));

                Debug.Assert(root.IsKind(SyntaxKind.CompilationUnit), root.Kind().ToString());

                if (root.IsKind(SyntaxKind.CompilationUnit))
                {
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
}