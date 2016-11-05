// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator
{
    public static class SyntaxTokenExtensions
    {
        public static SyntaxToken PrependLeadingTrivia(this SyntaxToken token, IEnumerable<SyntaxTrivia> trivia)
        {
            if (trivia == null)
                throw new ArgumentNullException(nameof(trivia));

            return token.WithLeadingTrivia(trivia.Concat(token.TrailingTrivia));
        }

        public static SyntaxToken PrependLeadingTrivia(this SyntaxToken token, SyntaxTrivia trivia)
        {
            return token.WithLeadingTrivia(token.LeadingTrivia.Insert(0, trivia));
        }

        public static SyntaxToken AppendTrailingTrivia(this SyntaxToken token, IEnumerable<SyntaxTrivia> trivia)
        {
            if (trivia == null)
                throw new ArgumentNullException(nameof(trivia));

            return token.WithTrailingTrivia(token.TrailingTrivia.AddRange(trivia));
        }

        public static SyntaxToken AppendTrailingTrivia(this SyntaxToken token, SyntaxTrivia trivia)
        {
            return token.WithTrailingTrivia(token.TrailingTrivia.Add(trivia));
        }

        public static IEnumerable<SyntaxTrivia> GetLeadingAndTrailingTrivia(this SyntaxToken token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            return token.LeadingTrivia.Concat(token.TrailingTrivia);
        }

        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2;
        }

        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3;
        }

        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4;
        }

        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5;
        }

        public static bool IsKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3, SyntaxKind kind4, SyntaxKind kind5, SyntaxKind kind6)
        {
            SyntaxKind kind = token.Kind();

            return kind == kind1
                || kind == kind2
                || kind == kind3
                || kind == kind4
                || kind == kind5
                || kind == kind6;
        }

        public static int GetSpanStartLine(this SyntaxToken token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (token.SyntaxTree != null)
                return token.SyntaxTree.GetLineSpan(token.Span, cancellationToken).StartLine();

            return -1;
        }

        public static int GetFullSpanStartLine(this SyntaxToken token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (token.SyntaxTree != null)
                return token.SyntaxTree.GetLineSpan(token.FullSpan, cancellationToken).StartLine();

            return -1;
        }

        public static int GetSpanEndLine(this SyntaxToken token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (token.SyntaxTree != null)
                return token.SyntaxTree.GetLineSpan(token.Span, cancellationToken).EndLine();

            return -1;
        }

        public static int GetFullSpanEndLine(this SyntaxToken token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (token.SyntaxTree != null)
                return token.SyntaxTree.GetLineSpan(token.FullSpan, cancellationToken).EndLine();

            return -1;
        }

        public static SyntaxToken TrimLeadingTrivia(this SyntaxToken token)
        {
            return token.WithLeadingTrivia(token.LeadingTrivia.TrimStart());
        }

        public static SyntaxToken TrimTrailingTrivia(this SyntaxToken token)
        {
            return token.WithTrailingTrivia(token.TrailingTrivia.TrimEnd());
        }

        public static SyntaxToken WithoutTrivia(this SyntaxToken token)
        {
            return token.WithoutLeadingTrivia().WithoutTrailingTrivia();
        }

        public static SyntaxToken WithoutLeadingTrivia(this SyntaxToken token)
        {
            return token.WithLeadingTrivia((IEnumerable<SyntaxTrivia>)null);
        }

        public static SyntaxToken WithoutTrailingTrivia(this SyntaxToken token)
        {
            return token.WithTrailingTrivia((IEnumerable<SyntaxTrivia>)null);
        }

        public static SyntaxToken WithTrailingSpace(this SyntaxToken token)
        {
            return token.WithTrailingTrivia(SyntaxFactory.Space);
        }

        public static SyntaxToken WithTrailingNewLine(this SyntaxToken token)
        {
            return token.WithTrailingTrivia(CSharpFactory.NewLineTrivia());
        }

        public static SyntaxToken WithFormatterAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(Formatter.Annotation);
        }

        public static SyntaxToken WithSimplifierAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(Simplifier.Annotation);
        }

        public static SyntaxToken WithRenameAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(RenameAnnotation.Create());
        }

        public static SyntaxTokenList ToSyntaxTokenList(this IEnumerable<SyntaxToken> tokens)
        {
            return TokenList(tokens);
        }
    }
}
