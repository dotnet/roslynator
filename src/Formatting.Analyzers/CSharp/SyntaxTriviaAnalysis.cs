// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    internal static class SyntaxTriviaAnalysis
    {
        public static bool IsEmptyOrSingleWhitespaceTrivia(SyntaxTriviaList triviaList)
        {
            int count = triviaList.Count;

            return count == 0
                || (count == 1 && triviaList[0].IsWhitespaceTrivia());
        }

        public static SyntaxTrivia GetEndOfLine(SyntaxNodeOrToken nodeOrToken)
        {
            if (nodeOrToken.IsNode)
            {
                return GetEndOfLine(nodeOrToken.AsNode());
            }
            else if (nodeOrToken.IsToken)
            {
                return GetEndOfLine(nodeOrToken.AsToken());
            }
            else
            {
                throw new ArgumentException("", nameof(nodeOrToken));
            }
        }

        public static SyntaxTrivia GetEndOfLine(SyntaxNode node)
        {
            return GetEndOfLine(node.GetFirstToken());
        }

        public static SyntaxTrivia GetEndOfLine(SyntaxToken token)
        {
            SyntaxTrivia trivia = FindEndOfLine(token);

            return (trivia.IsEndOfLineTrivia()) ? trivia : CSharpFactory.NewLine();
        }

        public static SyntaxTrivia FindEndOfLine(SyntaxNode node)
        {
            return FindEndOfLine(node.GetFirstToken());
        }

        public static SyntaxTrivia FindEndOfLine(SyntaxToken token)
        {
            SyntaxToken t = token;

            do
            {
                foreach (SyntaxTrivia trivia in t.LeadingTrivia)
                {
                    if (trivia.IsEndOfLineTrivia())
                        return trivia;
                }

                foreach (SyntaxTrivia trivia in t.TrailingTrivia)
                {
                    if (trivia.IsEndOfLineTrivia())
                        return trivia;
                }

                t = t.GetNextToken();

            } while (!t.IsKind(SyntaxKind.None));

            t = token;

            while (true)
            {
                t = t.GetPreviousToken();

                if (t.IsKind(SyntaxKind.None))
                    break;

                foreach (SyntaxTrivia trivia in t.LeadingTrivia)
                {
                    if (trivia.IsEndOfLineTrivia())
                        return trivia;
                }

                foreach (SyntaxTrivia trivia in t.TrailingTrivia)
                {
                    if (trivia.IsEndOfLineTrivia())
                        return trivia;
                }
            }

            return default;
        }

        public static bool IsTokenPrecededWithNewLineAndNotFollowedWithNewLine(
            ExpressionSyntax left,
            SyntaxToken token,
            ExpressionSyntax right)
        {
            return IsOptionalWhitespaceThenEndOfLineTrivia(left.GetTrailingTrivia())
                && token.LeadingTrivia.IsEmptyOrWhitespace()
                && token.TrailingTrivia.SingleOrDefault(shouldThrow: false).IsWhitespaceTrivia()
                && !right.GetLeadingTrivia().Any();
        }

        public static bool IsTokenFollowedWithNewLineAndNotPrecededWithNewLine(
            ExpressionSyntax left,
            SyntaxToken token,
            ExpressionSyntax right)
        {
            return left.GetTrailingTrivia().SingleOrDefault(shouldThrow: false).IsWhitespaceTrivia()
                && !token.LeadingTrivia.Any()
                && IsOptionalWhitespaceThenEndOfLineTrivia(token.TrailingTrivia)
                && right.GetLeadingTrivia().IsEmptyOrWhitespace();
        }

        public static bool IsOptionalWhitespaceThenEndOfLineTrivia(SyntaxTriviaList triviaList)
        {
            SyntaxTriviaList.Enumerator en = triviaList.GetEnumerator();

            if (!en.MoveNext())
                return false;

            SyntaxKind kind = en.Current.Kind();

            if (kind == SyntaxKind.WhitespaceTrivia)
            {
                if (!en.MoveNext())
                    return false;

                kind = en.Current.Kind();
            }

            return kind == SyntaxKind.EndOfLineTrivia
                && !en.MoveNext();
        }

        public static bool IsOptionalWhitespaceThenOptionalSingleLineCommentThenEndOfLineTrivia(SyntaxTriviaList triviaList)
        {
            SyntaxTriviaList.Enumerator en = triviaList.GetEnumerator();

            if (!en.MoveNext())
                return false;

            SyntaxKind kind = en.Current.Kind();

            if (kind == SyntaxKind.WhitespaceTrivia)
            {
                if (!en.MoveNext())
                    return false;

                kind = en.Current.Kind();
            }

            if (kind == SyntaxKind.SingleLineCommentTrivia)
            {
                if (!en.MoveNext())
                    return false;

                kind = en.Current.Kind();
            }

            return kind == SyntaxKind.EndOfLineTrivia
                && !en.MoveNext();
        }

        public static bool StartsWithOptionalWhitespaceThenEndOfLineTrivia(SyntaxTriviaList trivia)
        {
            SyntaxTriviaList.Enumerator en = trivia.GetEnumerator();

            if (!en.MoveNext())
                return false;

            if (en.Current.IsWhitespaceTrivia()
                && !en.MoveNext())
            {
                return false;
            }

            return en.Current.IsEndOfLineTrivia();
        }
    }
}
