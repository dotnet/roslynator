// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Pihrtsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis
{
    public static class SyntaxTokenExtensions
    {
        public static bool IsNoneKind(this SyntaxToken token)
        {
            return token.IsKind(SyntaxKind.None);
        }

        public static bool IsCommaToken(this SyntaxToken token)
        {
            return token.IsKind(SyntaxKind.CommaToken);
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

        public static SyntaxToken WithFormatterAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(Formatter.Annotation);
        }

        public static SyntaxToken WithSimplifierAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(Simplifier.Annotation);
        }

        public static SyntaxToken TrimLeadingTrivia(this SyntaxToken token)
            => token.WithLeadingTrivia(token.LeadingTrivia.TrimStart());

        public static SyntaxToken TrimTrailingTrivia(this SyntaxToken token)
            => token.WithTrailingTrivia(token.TrailingTrivia.TrimEnd());

        public static SyntaxToken WithoutLeadingTrivia(this SyntaxToken token)
            => token.WithLeadingTrivia(SyntaxTriviaList.Empty);

        public static SyntaxToken WithoutTrailingTrivia(this SyntaxToken token)
            => token.WithTrailingTrivia(SyntaxTriviaList.Empty);

        public static SyntaxToken WithTrailingSpace(this SyntaxToken token)
            => token.WithTrailingTrivia(SyntaxFactory.Space);

        public static SyntaxToken WithTrailingNewLine(this SyntaxToken token)
            => token.WithTrailingTrivia(CSharpFactory.NewLine);
    }
}
