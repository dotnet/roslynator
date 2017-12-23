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

        public static bool IsFixable(SyntaxTrivia trivia)
        {
            if (trivia.Kind() != SyntaxKind.SingleLineCommentTrivia)
                return false;

            if (!(trivia.Token.Parent is MemberDeclarationSyntax memberDeclaration))
                return false;

            if (trivia.SpanStart >= memberDeclaration.SpanStart)
                return false;

            SyntaxTriviaList leadingTrivia = memberDeclaration.GetLeadingTrivia();

            int i = leadingTrivia.IndexOf(trivia);

            Debug.Assert(i != -1, trivia.ToString());

            if (i == -1)
                return false;

            i++;

            while (i < leadingTrivia.Count)
            {
                if (!leadingTrivia[i].IsKind(
                    SyntaxKind.WhitespaceTrivia,
                    SyntaxKind.EndOfLineTrivia,
                    SyntaxKind.SingleLineCommentTrivia))
                {
                    return false;
                }

                i++;
            }

            return true;
        }

        public static TextSpan GetFixableSpan(MemberDeclarationSyntax memberDeclaration)
        {
            return GetFixableSpan(memberDeclaration.GetLeadingTrivia());
        }

        private static TextSpan GetFixableSpan(SyntaxTriviaList leadingTrivia)
        {
            if (leadingTrivia.Any())
            {
                var span = default(TextSpan);

                int i = leadingTrivia.Count - 1;
                while (i >= 0)
                {
                    if (leadingTrivia[i].IsWhitespaceTrivia())
                        i--;

                    if (i < 0)
                        break;

                    if (!leadingTrivia[i].IsEndOfLineTrivia())
                        break;

                    i--;

                    if (i < 0)
                        break;

                    SyntaxTrivia trivia = leadingTrivia[i];

                    if (!trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                        break;

                    span = (span == default(TextSpan))
                        ? trivia.Span
                        : TextSpan.FromBounds(trivia.SpanStart, span.End);

                    i--;
                }

                return span;
            }

            return default(TextSpan);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxTriviaList leadingTrivia = memberDeclaration.GetLeadingTrivia();

            TextSpan commentsSpan = GetFixableSpan(leadingTrivia);

            ImmutableArray<string> comments = leadingTrivia
                .Where(f => commentsSpan.Contains(f.Span) && f.IsKind(SyntaxKind.SingleLineCommentTrivia))
                .Select(f => _leadingSlashesRegex.Replace(f.ToString(), ""))
                .ToImmutableArray();

            TextSpan spanToRemove = TextSpan.FromBounds(commentsSpan.Start, memberDeclaration.SpanStart);

            MemberDeclarationSyntax newNode = memberDeclaration.WithLeadingTrivia(leadingTrivia.Where(f => !spanToRemove.Contains(f.Span)));

            var settings = new DocumentationCommentGeneratorSettings(comments);

            newNode = newNode.WithNewSingleLineDocumentationComment(settings);

            return document.ReplaceNodeAsync(memberDeclaration, newNode, cancellationToken);
        }
    }
}
