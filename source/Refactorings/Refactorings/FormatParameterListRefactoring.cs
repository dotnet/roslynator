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
    internal static class FormatParameterListRefactoring
    {
        public static async Task<Document> FormatEachParameterOnSeparateLineAsync(
            Document document,
            ParameterListSyntax parameterList,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = oldRoot.ReplaceNode(
                parameterList,
                CreateMultilineList(parameterList));

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> FormatAllParametersOnSingleLineAsync(
            Document document,
            ParameterListSyntax parameterList,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ParameterListSyntax newParameterList = SyntaxRemover.RemoveWhitespaceOrEndOfLine(parameterList)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(parameterList, newParameterList);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ParameterListSyntax CreateMultilineList(ParameterListSyntax list)
        {
            SeparatedSyntaxList<ParameterSyntax> parameters = SeparatedList<ParameterSyntax>(CreateNodesAndTokens(list));

            SyntaxToken openParen = Token(SyntaxKind.OpenParenToken).WithTrailingNewLine();

            return ParameterList(parameters).WithOpenParenToken(openParen);
        }

        private static IEnumerable<SyntaxNodeOrToken> CreateNodesAndTokens(ParameterListSyntax list)
        {
            SyntaxTriviaList trivia = SyntaxUtility.GetIndentTrivia(list.Parent).Add(CSharpFactory.IndentTrivia());

            SeparatedSyntaxList<ParameterSyntax>.Enumerator en = list.Parameters.GetEnumerator();

            if (en.MoveNext())
            {
                yield return en.Current.WithLeadingTrivia(trivia);

                while (en.MoveNext())
                {
                    yield return Token(SyntaxKind.CommaToken).WithTrailingNewLine();

                    yield return en.Current.WithLeadingTrivia(trivia);
                }
            }
        }
    }
}
