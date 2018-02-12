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

        public static TextSpan GetFixableSpan(SyntaxTrivia trivia)
        {
            if (trivia.Kind() != SyntaxKind.SingleLineCommentTrivia)
                return default(TextSpan);

            if (!(trivia.Token.Parent is MemberDeclarationSyntax memberDeclaration))
                return default(TextSpan);

            TextSpan span = trivia.Span;

            if (memberDeclaration.LeadingTriviaSpan().Contains(span)
                || memberDeclaration.TrailingTriviaSpan().Contains(span))
            {
                (TextSpan fixableSpan, bool hasDocumentationComment) = AnalyzeLeadingTrivia(memberDeclaration);

                if (fixableSpan.Contains(span))
                    return fixableSpan;
            }

            return default(TextSpan);
        }

        public static (TextSpan span, bool hasDocumentationComment) AnalyzeLeadingTrivia(SyntaxNode declaration)
        {
            SyntaxTriviaList leadingTrivia = declaration.GetLeadingTrivia();

            SyntaxTriviaList.Reversed.Enumerator en = leadingTrivia.Reverse().GetEnumerator();

            var span = default(TextSpan);

            while (en.MoveNext())
            {
                if (en.Current.Kind() == SyntaxKind.WhitespaceTrivia
                    && !en.MoveNext())
                {
                    break;
                }

                if (en.Current.Kind() == SyntaxKind.EndOfLineTrivia)
                {
                    if (!en.MoveNext())
                        break;

                    if (en.Current.Kind() == SyntaxKind.SingleLineCommentTrivia)
                    {
                        span = (span == default(TextSpan))
                            ? en.Current.Span
                            : TextSpan.FromBounds(en.Current.SpanStart, span.End);
                    }
                }
                else
                {
                    do
                    {
                        if (SyntaxFacts.IsDocumentationCommentTrivia(en.Current.Kind()))
                            return (default(TextSpan), true);

                    } while (en.MoveNext());

                    break;
                }
            }

            return (span, false);
        }

        public static (TextSpan span, bool containsEndOfLine) AnalyzeTrailingTrivia(SyntaxNode declaration)
        {
            if (declaration == null)
                return (default(TextSpan), false);

            return AnalyzeTrailingTrivia(declaration.GetTrailingTrivia());
        }

        public static (TextSpan span, bool containsEndOfLine) AnalyzeTrailingTrivia<TNode>(SyntaxList<TNode> nodes) where TNode : SyntaxNode
        {
            TNode node = nodes.FirstOrDefault();

            if (node == null)
                return (default(TextSpan), false);

            return AnalyzeTrailingTrivia(node.GetTrailingTrivia());
        }

        public static (TextSpan span, bool containsEndOfLine) AnalyzeTrailingTrivia(SyntaxToken token)
        {
            return AnalyzeTrailingTrivia(token.TrailingTrivia);
        }

        public static (TextSpan span, bool containsEndOfLine) AnalyzeTrailingTrivia(SyntaxTriviaList trailingTrivia)
        {
            SyntaxTriviaList.Enumerator en = trailingTrivia.GetEnumerator();

            if (en.MoveNext())
            {
                SyntaxKind kind = en.Current.Kind();

                if (kind == SyntaxKind.WhitespaceTrivia)
                {
                    if (en.MoveNext())
                    {
                        if (en.Current.Kind() == SyntaxKind.SingleLineCommentTrivia)
                        {
                            return (en.Current.Span, false);
                        }
                        else
                        {
                            while (en.MoveNext())
                            {
                                if (en.Current.IsEndOfLineTrivia())
                                    return (default(TextSpan), true);
                            }
                        }
                    }
                }
                else if (kind == SyntaxKind.EndOfLineTrivia)
                {
                    return (default(TextSpan), true);
                }
                else if (kind == SyntaxKind.SingleLineCommentTrivia)
                {
                    return (en.Current.Span, false);
                }
                else
                {
                    while (en.MoveNext())
                    {
                        if (en.Current.IsEndOfLineTrivia())
                            return (default(TextSpan), true);
                    }
                }
            }

            return (default(TextSpan), false);
        }

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
                    .Where(f => span.Contains(f.Span) && f.IsKind(SyntaxKind.SingleLineCommentTrivia))
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
