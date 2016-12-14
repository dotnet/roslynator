// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatParameterListRefactoring
    {
        public static async Task<Document> FormatEachParameterOnSeparateLineAsync(
            Document document,
            ParameterListSyntax parameterList,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await document.ReplaceNodeAsync(
                parameterList,
                CreateMultilineList(parameterList),
                cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> FormatAllParametersOnSingleLineAsync(
            Document document,
            ParameterListSyntax parameterList,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ParameterListSyntax newParameterList = SyntaxRemover.RemoveWhitespaceOrEndOfLine(parameterList)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(parameterList, newParameterList, cancellationToken).ConfigureAwait(false);
        }

        private static ParameterListSyntax CreateMultilineList(ParameterListSyntax list)
        {
            SeparatedSyntaxList<ParameterSyntax> parameters = SeparatedList<ParameterSyntax>(CreateNodesAndTokens(list));

            SyntaxToken openParen = OpenParenToken().WithTrailingNewLine();

            return ParameterList(parameters).WithOpenParenToken(openParen);
        }

        private static IEnumerable<SyntaxNodeOrToken> CreateNodesAndTokens(ParameterListSyntax list)
        {
            SyntaxTriviaList trivia = SyntaxHelper.GetIndentTrivia(list.Parent).Add(IndentTrivia());

            SeparatedSyntaxList<ParameterSyntax>.Enumerator en = list.Parameters.GetEnumerator();

            if (en.MoveNext())
            {
                yield return en.Current.WithLeadingTrivia(trivia);

                while (en.MoveNext())
                {
                    yield return CommaToken().WithTrailingNewLine();

                    yield return en.Current.WithLeadingTrivia(trivia);
                }
            }
        }
    }
}
