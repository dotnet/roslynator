// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MoveUsingDirectiveToTopLevelRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            NamespaceDeclarationSyntax namespaceDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var compilationUnit = (CompilationUnitSyntax)root;

            SyntaxList<UsingDirectiveSyntax> usings = namespaceDeclaration.Usings;

            CompilationUnitSyntax newCompilationUnit = compilationUnit
                .RemoveNodes(usings, SyntaxRemoveOptions.KeepUnbalancedDirectives);

            if (!compilationUnit.Usings.Any())
            {
                SyntaxTriviaList leadingTrivia = compilationUnit.GetLeadingTrivia();

                SyntaxTrivia[] topTrivia = GetTopSingleLineComments(leadingTrivia).ToArray();

                if (topTrivia.Length > 0)
                {
                    newCompilationUnit = newCompilationUnit.WithoutLeadingTrivia();

                    usings = usings.Replace(
                        usings.First(),
                        usings.First().WithLeadingTrivia(topTrivia));

                    usings = usings.Replace(
                        usings.Last(),
                        usings.Last().WithTrailingTrivia(leadingTrivia.Skip(topTrivia.Length)));
                }
            }

            newCompilationUnit = newCompilationUnit.AddUsings(usings.Select(f => f.WithFormatterAnnotation()));

            return document.WithSyntaxRoot(newCompilationUnit);
        }

        private static IEnumerable<SyntaxTrivia> GetTopSingleLineComments(SyntaxTriviaList triviaList)
        {
            SyntaxTriviaList.Enumerator en = triviaList.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current.IsSingleLineCommentTrivia())
                {
                    SyntaxTrivia trivia = en.Current;

                    if (en.MoveNext() && en.Current.IsEndOfLineTrivia())
                    {
                        yield return trivia;
                        yield return en.Current;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
