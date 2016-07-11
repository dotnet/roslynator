// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis
{
    public static class SyntaxTriviaExtensions
    {
        public static bool IsCommentTrivia(this SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    return true;
                default:
                    return false;
            }
        }

        public static int GetSpanStartLine(this SyntaxTrivia trivia, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (trivia.SyntaxTree != null)
                return trivia.SyntaxTree.GetLineSpan(trivia.Span, cancellationToken).StartLinePosition.Line;

            return -1;
        }

        public static int GetFullSpanStartLine(this SyntaxTrivia trivia, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (trivia.SyntaxTree != null)
                return trivia.SyntaxTree.GetLineSpan(trivia.FullSpan, cancellationToken).StartLinePosition.Line;

            return -1;
        }

        public static int GetSpanEndLine(this SyntaxTrivia trivia, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (trivia.SyntaxTree != null)
                return trivia.SyntaxTree.GetLineSpan(trivia.Span, cancellationToken).EndLinePosition.Line;

            return -1;
        }

        public static int GetFullSpanEndLine(this SyntaxTrivia trivia, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (trivia.SyntaxTree != null)
                return trivia.SyntaxTree.GetLineSpan(trivia.FullSpan, cancellationToken).EndLinePosition.Line;

            return -1;
        }

        public static bool IsAnyKind(this SyntaxTrivia trivia, SyntaxKind kind, SyntaxKind kind2)
        {
            return trivia.IsKind(kind) || trivia.IsKind(kind2);
        }

        public static bool IsAnyKind(this SyntaxTrivia syntaxTrivia, IEnumerable<SyntaxKind> syntaxKinds)
        {
            if (syntaxKinds == null)
                throw new ArgumentNullException(nameof(syntaxKinds));

            foreach (SyntaxKind syntaxKind in syntaxKinds)
            {
                if (syntaxTrivia.IsKind(syntaxKind))
                    return true;
            }

            return false;
        }

        public static bool IsWhitespaceOrEndOfLineTrivia(this SyntaxTrivia syntaxTrivia)
        {
            if (syntaxTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                return true;

            if (syntaxTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                return true;

            return false;
        }

        public static bool IsWhitespaceTrivia(this SyntaxTrivia syntaxTrivia)
            => syntaxTrivia.IsKind(SyntaxKind.WhitespaceTrivia);

        public static bool IsEndOfLineTrivia(this SyntaxTrivia syntaxTrivia)
            => syntaxTrivia.IsKind(SyntaxKind.EndOfLineTrivia);
    }
}
