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
using Roslynator.Documentation;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertCommentToDocumentationCommentRefactoring
    {
        public const string Title = "Convert comment to documentation comment";

        private static readonly Regex _leadingSlashesRegex = new Regex(@"^//\s*");

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            TextSpan span,
            CancellationToken cancellationToken)
        {
            if (declaration is EnumDeclarationSyntax enumDeclaration
                && span.Start > enumDeclaration.Members.FirstOrDefault()?.SpanStart)
            {
                return RefactorAsync(document, enumDeclaration, span, cancellationToken);
            }

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

                comments = ImmutableArray.Create(_leadingSlashesRegex.Replace(trivia.ToString(), ""));

                SyntaxToken newToken = token.WithTrailingTrivia(trailingTrivia.Skip(trailingTrivia.IndexOf(trivia) + 1));

                newDeclaration = newDeclaration.ReplaceToken(token, newToken);
            }

            var settings = new DocumentationCommentGeneratorSettings(comments);

            newDeclaration = newDeclaration.WithNewSingleLineDocumentationComment(settings);

            return document.ReplaceNodeAsync(declaration, newDeclaration, cancellationToken);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            TextSpan span,
            CancellationToken cancellationToken)
        {
            SyntaxTrivia trivia = enumDeclaration.FindTrivia(span.Start);

            SyntaxToken token = trivia.Token;

            EnumMemberDeclarationSyntax enumMemberDeclaration = token
                .GetPreviousToken()
                .Parent
                .FirstAncestor<EnumMemberDeclarationSyntax>();

            int enumMemberIndex = enumDeclaration.Members.IndexOf(enumMemberDeclaration);

            SyntaxTriviaList trailingTrivia = token.TrailingTrivia;

            SyntaxToken newToken = token.WithTrailingTrivia(trailingTrivia.Skip(trailingTrivia.IndexOf(trivia) + 1));

            EnumDeclarationSyntax newEnumDeclaration = enumDeclaration.ReplaceToken(token, newToken);

            var settings = new DocumentationCommentGeneratorSettings(ImmutableArray.Create(_leadingSlashesRegex.Replace(trivia.ToString(), "")));

            EnumMemberDeclarationSyntax newEnumMemberDeclaration = newEnumDeclaration.Members[enumMemberIndex].WithNewSingleLineDocumentationComment(settings);

            newEnumDeclaration = newEnumDeclaration.WithMembers(newEnumDeclaration.Members.ReplaceAt(enumMemberIndex, newEnumMemberDeclaration));

            return document.ReplaceNodeAsync(enumDeclaration, newEnumDeclaration, cancellationToken);
        }
    }
}
