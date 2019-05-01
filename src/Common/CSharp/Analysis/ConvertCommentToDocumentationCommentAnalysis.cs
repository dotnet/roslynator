// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    internal static class ConvertCommentToDocumentationCommentAnalysis
    {
        public static TextSpan GetFixableSpan(SyntaxTrivia trivia)
        {
            if (trivia.Kind() != SyntaxKind.SingleLineCommentTrivia)
                return default;

            if (!(trivia.Token.Parent is MemberDeclarationSyntax memberDeclaration))
                return default;

            TextSpan span = trivia.Span;

            if (memberDeclaration.LeadingTriviaSpan().Contains(span)
                || memberDeclaration.TrailingTriviaSpan().Contains(span))
            {
                LeadingAnalysis analysis = AnalyzeLeadingTrivia(memberDeclaration);

                if (analysis.Span.Contains(span))
                    return analysis.Span;
            }

            return default;
        }

        public static LeadingAnalysis AnalyzeLeadingTrivia(SyntaxNode declaration)
        {
            SyntaxTriviaList leadingTrivia = declaration.GetLeadingTrivia();

            SyntaxTriviaList.Reversed.Enumerator en = leadingTrivia.Reverse().GetEnumerator();

            TextSpan span = default;

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
                        if (span == default)
                        {
                            if (declaration.SyntaxTree.Options.DocumentationMode == DocumentationMode.None
                                && en.Current.ToString().StartsWith("///"))
                            {
                                break;
                            }

                            span = en.Current.Span;
                        }
                        else
                        {
                            span = TextSpan.FromBounds(en.Current.SpanStart, span.End);
                        }
                    }
                }
                else
                {
                    do
                    {
                        if (SyntaxFacts.IsDocumentationCommentTrivia(en.Current.Kind()))
                            return new LeadingAnalysis(default, true);

                    } while (en.MoveNext());

                    break;
                }
            }

            return new LeadingAnalysis(span, false);
        }

        public static TrailingAnalysis AnalyzeTrailingTrivia(SyntaxNode declaration)
        {
            if (declaration == null)
                return default;

            return AnalyzeTrailingTrivia(declaration.GetTrailingTrivia());
        }

        public static TrailingAnalysis AnalyzeTrailingTrivia<TNode>(SyntaxList<TNode> nodes) where TNode : SyntaxNode
        {
            TNode node = nodes.FirstOrDefault();

            if (node == null)
                return default;

            return AnalyzeTrailingTrivia(node.GetTrailingTrivia());
        }

        public static TrailingAnalysis AnalyzeTrailingTrivia(SyntaxToken token)
        {
            return AnalyzeTrailingTrivia(token.TrailingTrivia);
        }

        public static TrailingAnalysis AnalyzeTrailingTrivia(SyntaxTriviaList trailingTrivia)
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
                            return new TrailingAnalysis(en.Current.Span, false);
                        }
                        else
                        {
                            while (en.MoveNext())
                            {
                                if (en.Current.IsEndOfLineTrivia())
                                    return new TrailingAnalysis(default, true);
                            }
                        }
                    }
                }
                else if (kind == SyntaxKind.EndOfLineTrivia)
                {
                    return new TrailingAnalysis(default, true);
                }
                else if (kind == SyntaxKind.SingleLineCommentTrivia)
                {
                    return new TrailingAnalysis(en.Current.Span, false);
                }
                else
                {
                    while (en.MoveNext())
                    {
                        if (en.Current.IsEndOfLineTrivia())
                            return new TrailingAnalysis(default(TextSpan), true);
                    }
                }
            }

            return default;
        }

        public readonly struct LeadingAnalysis
        {
            public LeadingAnalysis(TextSpan span, bool hasDocumentationComment)
            {
                Span = span;
                HasDocumentationComment = hasDocumentationComment;
            }

            public TextSpan Span { get; }

            public bool HasDocumentationComment { get; }
        }

        public readonly struct TrailingAnalysis
        {
            public TrailingAnalysis(TextSpan span, bool containsEndOfLine)
            {
                Span = span;
                ContainsEndOfLine = containsEndOfLine;
            }

            public TextSpan Span { get; }

            public bool ContainsEndOfLine { get; }
        }
    }
}
