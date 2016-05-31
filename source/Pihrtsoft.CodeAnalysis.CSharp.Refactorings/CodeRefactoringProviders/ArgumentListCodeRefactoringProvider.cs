// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ArgumentListCodeRefactoringProvider))]
    public class ArgumentListCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            ArgumentListSyntax argumentList = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ArgumentListSyntax>();

            if (argumentList == null)
                return;

            if (argumentList.Arguments.Count == 0)
                return;

            if (!context.Span.IsEmpty)
                ArgumentRefactoring.AddOrRemoveArgumentName(context, argumentList);

            if (argumentList.IsSingleline())
            {
                if (argumentList.Arguments.Count > 1)
                {
                    context.RegisterRefactoring(
                        "Format each argument on separate line",
                        cancellationToken => FormatEachArgumentOnNewLineAsync(context.Document, argumentList, cancellationToken));
                }
            }
            else
            {
                context.RegisterRefactoring(
                    "Format all arguments on a single line",
                    cancellationToken => FormatAllArgumentsOnSingleLineAsync(context.Document, argumentList, cancellationToken));
            }

            SwapArgumentsRefactoring.Refactor(context, argumentList);
        }

        private static async Task<Document> FormatEachArgumentOnNewLineAsync(
            Document document,
            ArgumentListSyntax argumentList,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newRoot = oldRoot.ReplaceNode(
                argumentList,
                CreateMultilineList(argumentList));

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> FormatAllArgumentsOnSingleLineAsync(
            Document document,
            ArgumentListSyntax argumentList,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ArgumentListSyntax newArgumentList = WhitespaceOrEndOfLineRemover.RemoveFrom(argumentList)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(argumentList, newArgumentList);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ArgumentListSyntax CreateMultilineList(ArgumentListSyntax argumentList)
        {
            SeparatedSyntaxList<ArgumentSyntax> arguments = SeparatedList<ArgumentSyntax>(CreateMultilineNodesAndTokens(argumentList));

            SyntaxToken openParen = Token(SyntaxKind.OpenParenToken)
                .WithTrailingNewLine();

            return ArgumentList(arguments)
                .WithOpenParenToken(openParen)
                .WithCloseParenToken(argumentList.CloseParenToken.WithoutLeadingTrivia());
        }

        private static IEnumerable<SyntaxNodeOrToken> CreateMultilineNodesAndTokens(ArgumentListSyntax argumentList)
        {
            SyntaxTriviaList trivia = argumentList.Parent.GetIndentTrivia().Add(SyntaxHelper.DefaultIndent);

            SeparatedSyntaxList<ArgumentSyntax>.Enumerator en = argumentList.Arguments.GetEnumerator();

            if (en.MoveNext())
            {
                yield return en.Current
                    .TrimTrailingWhitespace()
                    .WithLeadingTrivia(trivia);

                while (en.MoveNext())
                {
                    yield return Token(SyntaxKind.CommaToken)
                        .WithTrailingNewLine();

                    yield return en.Current
                        .TrimTrailingWhitespace()
                        .WithLeadingTrivia(trivia);
                }
            }
        }
    }
}