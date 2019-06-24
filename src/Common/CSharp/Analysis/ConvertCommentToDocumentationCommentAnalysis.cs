// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
            bool containsTaskListItem = false;
            bool containsNonTaskListItem = false;

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
                                return new LeadingAnalysis(default, true, containsTaskListItem, containsNonTaskListItem);
                            }

                            span = en.Current.Span;
                        }
                        else
                        {
                            span = TextSpan.FromBounds(en.Current.SpanStart, span.End);
                        }

                        if (!containsTaskListItem
                            || !containsNonTaskListItem)
                        {
                            if (IsTaskListItem(en.Current))
                            {
                                containsTaskListItem = true;
                            }
                            else
                            {
                                containsNonTaskListItem = true;
                            }
                        }
                    }
                }
                else
                {
                    do
                    {
                        if (SyntaxFacts.IsDocumentationCommentTrivia(en.Current.Kind()))
                            return new LeadingAnalysis(default, true, containsTaskListItem, containsNonTaskListItem);

                    } while (en.MoveNext());

                    break;
                }
            }

            return new LeadingAnalysis(span, false, containsTaskListItem, containsNonTaskListItem);
        }

        public static TrailingAnalysis AnalyzeTrailingTrivia(SyntaxNodeOrToken nodeOrToken)
        {
            SyntaxTriviaList.Enumerator en = nodeOrToken.GetTrailingTrivia().GetEnumerator();

            if (en.MoveNext())
            {
                SyntaxKind kind = en.Current.Kind();

                if (kind == SyntaxKind.WhitespaceTrivia)
                {
                    if (en.MoveNext())
                    {
                        if (en.Current.Kind() == SyntaxKind.SingleLineCommentTrivia)
                        {
                            return (IsTaskListItem(en.Current))
                                ? new TrailingAnalysis(default, false)
                                : new TrailingAnalysis(en.Current.Span, false);
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
                    return (IsTaskListItem(en.Current))
                        ? new TrailingAnalysis(default, false)
                        : new TrailingAnalysis(en.Current.Span, false);
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

            return default;
        }

        private static bool IsTaskListItem(SyntaxTrivia singleLineComment)
        {
            string s = singleLineComment.ToString();

            int len = s.Length;

            int i = 0;

            while (i < len
                && s[i] == '/')
            {
                i++;
            }

            while (i < len
                && char.IsWhiteSpace(s[i]))
            {
                i++;
            }

            int startIndex = i;

            while (i < len
                && char.IsLetter(s[i]))
            {
                i++;
            }

            int length = i - startIndex;

            if (length < 4)
                return false;

            return string.Compare(s, startIndex, "todo", 0, length, StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(s, startIndex, "hack", 0, length, StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(s, startIndex, "undone", 0, length, StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(s, startIndex, "unresolvedmergeconflict", 0, length, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public readonly struct LeadingAnalysis
        {
            public LeadingAnalysis(
                TextSpan span,
                bool hasDocumentationComment,
                bool containsTaskListItem,
                bool containsNonTaskListItem)
            {
                Span = span;
                HasDocumentationComment = hasDocumentationComment;
                ContainsTaskListItem = containsTaskListItem;
                ContainsNonTaskListItem = containsNonTaskListItem;
            }

            public TextSpan Span { get; }
            public bool HasDocumentationComment { get; }
            public bool ContainsTaskListItem { get; }
            public bool ContainsNonTaskListItem { get; }
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
