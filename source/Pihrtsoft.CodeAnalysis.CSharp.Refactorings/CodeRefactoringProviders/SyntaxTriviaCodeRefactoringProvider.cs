// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis;
using Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(SyntaxTriviaCodeRefactoringProvider))]
    public class SyntaxTriviaCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            SyntaxTrivia trivia = root.FindTrivia(context.Span.Start);

            if (IsComment(trivia))
            {
                context.RegisterRefactoring(
                    "Remove comment",
                    cancellationToken => DeleteCommentAsync(context.Document, trivia, cancellationToken));

                Debug.Assert(root.IsKind(SyntaxKind.CompilationUnit), root.Kind().ToString());

                if (root.IsKind(SyntaxKind.CompilationUnit))
                {
                    context.RegisterRefactoring(
                        "Remove all comments",
                        cancellationToken => RemoveAllCommentsAsync(context.Document, cancellationToken));

                    if (trivia.IsAnyKind(SyntaxKind.SingleLineCommentTrivia, SyntaxKind.MultiLineCommentTrivia))
                    {
                        context.RegisterRefactoring(
                            "Remove all comments (except xml comments)",
                            cancellationToken => RemoveAllCommentsExceptXmlCommentsAsync(context.Document, cancellationToken));
                    }
                }
            }
        }

        private static bool IsComment(SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
                case SyntaxKind.SingleLineCommentTrivia:
                    return true;
                case SyntaxKind.MultiLineCommentTrivia:
                    return true;
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    return true;
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    return true;
                default:
                    return false;
            }
        }

        private static async Task<Document> RemoveAllCommentsAsync(
            Document document,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            CompilationUnitSyntax newRoot = RemoveAllCommentsSyntaxRewriter.VisitNode((CompilationUnitSyntax)oldRoot)
                .WithAdditionalAnnotations(Formatter.Annotation);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> RemoveAllCommentsExceptXmlCommentsAsync(
            Document document,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            CompilationUnitSyntax newRoot = RemoveAllCommentsSyntaxRewriter.VisitNode((CompilationUnitSyntax)oldRoot, keepXmlComment: true)
                .WithAdditionalAnnotations(Formatter.Annotation);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> DeleteCommentAsync(
            Document document,
            SyntaxTrivia comment,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxToken newToken = GetNewToken(comment.Token, comment)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceToken(comment.Token, newToken);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxToken GetNewToken(SyntaxToken token, SyntaxTrivia comment)
        {
            int index = token.LeadingTrivia.IndexOf(comment);

            if (index != -1)
            {
                if (comment.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    return token.WithLeadingTrivia(RemoveTrivia(token.LeadingTrivia, index));

                return token.WithLeadingTrivia(token.LeadingTrivia.RemoveAt(index));
            }

            index = token.TrailingTrivia.IndexOf(comment);

            if (index != -1)
            {
                if (comment.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    return token.WithTrailingTrivia(RemoveTrivia(token.TrailingTrivia, index));

                return token.WithTrailingTrivia(token.TrailingTrivia.RemoveAt(index));
            }

            Debug.Assert(false, "comment trivia not found");

            return token;
        }

        private static SyntaxTriviaList RemoveTrivia(SyntaxTriviaList triviaList, int index)
        {
            int first = FindFirstTriviaToRemove(triviaList, index);
            int last = FindLastTriviaToRemove(triviaList, index);

            for (int i = last; i >= first; i--)
                triviaList = triviaList.RemoveAt(i);

            return triviaList;
        }

        private static int FindFirstTriviaToRemove(SyntaxTriviaList triviaList, int index)
        {
            int firstIndex = index;

            while (index > 0)
            {
                if (IsAllowedTrivia(triviaList[index - 1]))
                {
                    index--;

                    if (triviaList[index].IsKind(SyntaxKind.SingleLineCommentTrivia))
                        firstIndex = index;
                }
                else
                {
                    break;
                }
            }

            return firstIndex;
        }

        private static int FindLastTriviaToRemove(SyntaxTriviaList triviaList, int index)
        {
            int lastIndex = index;

            while (index < triviaList.Count - 1)
            {
                if (IsAllowedTrivia(triviaList[index + 1]))
                {
                    index++;

                    if (triviaList[index].IsKind(SyntaxKind.SingleLineCommentTrivia))
                        lastIndex = index;
                }
                else
                {
                    break;
                }
            }

            return lastIndex;
        }

        private static bool IsAllowedTrivia(SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
                case SyntaxKind.WhitespaceTrivia:
                    return true;
                case SyntaxKind.EndOfLineTrivia:
                    return true;
                case SyntaxKind.SingleLineCommentTrivia:
                    return true;
                default:
                    return false;
            }
        }
    }
}