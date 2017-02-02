// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Extensions
{
    public static class SyntaxTokenExtensions
    {
        public static bool IsAccessModifier(this SyntaxToken token)
        {
            switch (token.Kind())
            {
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PrivateKeyword:
                    return true;
                default:
                    return false;
            }
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

        public static SyntaxToken TrimLeadingTrivia(this SyntaxToken token)
        {
            SyntaxTriviaList leadingTrivia = token.LeadingTrivia;
            SyntaxTriviaList newLeadingTrivia = leadingTrivia.TrimStart();

            if (leadingTrivia.Count != newLeadingTrivia.Count)
            {
                return token.WithLeadingTrivia(newLeadingTrivia);
            }
            else
            {
                return token;
            }
        }

        public static SyntaxToken TrimTrailingTrivia(this SyntaxToken token)
        {
            SyntaxTriviaList trailingTrivia = token.TrailingTrivia;
            SyntaxTriviaList newTrailingTrivia = trailingTrivia.TrimEnd();

            if (trailingTrivia.Count != newTrailingTrivia.Count)
            {
                return token.WithTrailingTrivia(newTrailingTrivia);
            }
            else
            {
                return token;
            }
        }

        public static SyntaxToken WithTrailingNewLine(this SyntaxToken token)
        {
            return token.WithTrailingTrivia(NewLineTrivia());
        }

        public static bool Contains(this SyntaxTokenList tokenList, SyntaxKind kind)
        {
            return tokenList.IndexOf(kind) != -1;
        }

        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind)
        {
            return Microsoft.CodeAnalysis.CSharpExtensions.IsKind(token.Parent, kind);
        }

        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2)
        {
            return SyntaxNodeExtensions.IsKind(token.Parent, kind1, kind2);
        }

        public static bool IsParentKind(this SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2, SyntaxKind kind3)
        {
            return SyntaxNodeExtensions.IsKind(token.Parent, kind1, kind2, kind3);
        }
    }
}
