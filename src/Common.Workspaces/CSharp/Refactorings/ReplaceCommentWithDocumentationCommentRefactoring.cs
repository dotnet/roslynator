// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Documentation;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceCommentWithDocumentationCommentRefactoring
    {
        public const string Title = "Replace comment with documentation comment";

        private static readonly Regex _leadingSlashesRegex = new Regex(@"^//\s*");

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            TextSpan span,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newDeclaration = declaration;

            ImmutableArray<string> comments;

            SyntaxTriviaList leadingTrivia = declaration.GetLeadingTrivia();

            if (leadingTrivia.Span.Contains(span))
            {
                comments = leadingTrivia
                    .Where(f => span.Contains(f.Span) && f.Kind() == SyntaxKind.SingleLineCommentTrivia)
                    .Select(f => _leadingSlashesRegex.Replace(f.ToString(), ""))
                    .ToImmutableArray();

                TextSpan spanToRemove = TextSpan.FromBounds(span.Start, declaration.SpanStart);

                newDeclaration = declaration.WithLeadingTrivia(leadingTrivia.Where(f => !spanToRemove.Contains(f.Span)));
            }
            else
            {
                SyntaxTrivia trivia = declaration.FindTrivia(span.Start);

                Debug.Assert(trivia != default(SyntaxTrivia));

                SyntaxToken token = trivia.Token;

                SyntaxTriviaList trailingTrivia = token.TrailingTrivia;

                Debug.Assert(trailingTrivia.Contains(trivia));

                for (int i = 0; i < trailingTrivia.Count; i++)
                {
                    if (trailingTrivia[i].Span == span)
                    {
                        comments = ImmutableArray.Create(_leadingSlashesRegex.Replace(trailingTrivia[i].ToString(), ""));

                        SyntaxToken newToken = token.WithTrailingTrivia(trailingTrivia.Skip(i + 1));

                        newDeclaration = newDeclaration.ReplaceToken(token, newToken);
                        break;
                    }
                }
            }

            var settings = new DocumentationCommentGeneratorSettings(comments);

            newDeclaration = newDeclaration.WithNewSingleLineDocumentationComment(settings);

            return document.ReplaceNodeAsync(declaration, newDeclaration, cancellationToken);
        }
    }
}
