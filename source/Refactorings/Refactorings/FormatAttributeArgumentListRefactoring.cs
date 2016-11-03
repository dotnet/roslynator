// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatAttributeArgumentListRefactoring
    {
        public static async Task<Document> FormatEachArgumentOnSeparateLineAsync(
            Document document,
            AttributeArgumentListSyntax argumentList,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = oldRoot.ReplaceNode(
                argumentList,
                CreateMultilineList(argumentList));

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> FormatAllArgumentsOnSingleLineAsync(
            Document document,
            AttributeArgumentListSyntax argumentList,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            AttributeArgumentListSyntax newArgumentList = SyntaxRemover.RemoveWhitespaceOrEndOfLine(argumentList)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(argumentList, newArgumentList);

            return document.WithSyntaxRoot(newRoot);
        }

        private static AttributeArgumentListSyntax CreateMultilineList(AttributeArgumentListSyntax argumentList)
        {
            SeparatedSyntaxList<AttributeArgumentSyntax> arguments = SeparatedList<AttributeArgumentSyntax>(CreateMultilineNodesAndTokens(argumentList));

            SyntaxToken openParen = Token(SyntaxKind.OpenParenToken)
                .WithTrailingNewLine();

            return AttributeArgumentList(arguments)
                .WithOpenParenToken(openParen)
                .WithCloseParenToken(argumentList.CloseParenToken.WithoutLeadingTrivia());
        }

        private static IEnumerable<SyntaxNodeOrToken> CreateMultilineNodesAndTokens(AttributeArgumentListSyntax argumentList)
        {
            SyntaxTriviaList trivia = SyntaxUtility.GetIndentTrivia(argumentList.Parent).Add(CSharpFactory.IndentTrivia());

            SeparatedSyntaxList<AttributeArgumentSyntax>.Enumerator en = argumentList.Arguments.GetEnumerator();

            if (en.MoveNext())
            {
                yield return en.Current
                    .TrimTrailingTrivia()
                    .WithLeadingTrivia(trivia);

                while (en.MoveNext())
                {
                    yield return Token(SyntaxKind.CommaToken)
                        .WithTrailingNewLine();

                    yield return en.Current
                        .TrimTrailingTrivia()
                        .WithLeadingTrivia(trivia);
                }
            }
        }
    }
}
