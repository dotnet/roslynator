// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.CSharp
{
    internal static class SyntaxTriviaAnalysis
    {
        public static bool IsExteriorTriviaEmptyOrWhitespace(SyntaxNode node)
        {
            return node.GetLeadingTrivia().IsEmptyOrWhitespace()
                && node.GetTrailingTrivia().IsEmptyOrWhitespace();
        }

        public static bool IsExteriorTriviaEmptyOrWhitespace(SyntaxToken token)
        {
            return token.LeadingTrivia.IsEmptyOrWhitespace()
                && token.TrailingTrivia.IsEmptyOrWhitespace();
        }

        public static bool IsEmptyOrSingleWhitespaceTrivia(SyntaxTriviaList triviaList)
        {
            int count = triviaList.Count;

            return count == 0
                || (count == 1 && triviaList[0].IsWhitespaceTrivia());
        }

        public static SyntaxTrivia DetermineEndOfLine(SyntaxNodeOrToken nodeOrToken, SyntaxTrivia? defaultValue = null)
        {
            if (nodeOrToken.IsNode)
            {
                return DetermineEndOfLine(nodeOrToken.AsNode(), defaultValue);
            }
            else if (nodeOrToken.IsToken)
            {
                return DetermineEndOfLine(nodeOrToken.AsToken(), defaultValue);
            }
            else
            {
                throw new ArgumentException("", nameof(nodeOrToken));
            }
        }

        public static SyntaxTrivia DetermineEndOfLine(SyntaxNode node, SyntaxTrivia? defaultValue = null)
        {
            return DetermineEndOfLine(node.GetFirstToken(), defaultValue);
        }

        public static SyntaxTrivia DetermineEndOfLine(SyntaxToken token, SyntaxTrivia? defaultValue = null)
        {
            SyntaxTrivia trivia = FindEndOfLine(token);

            return (trivia.IsEndOfLineTrivia())
                ? trivia
                : defaultValue ?? CSharpFactory.NewLine();
        }

        private static SyntaxTrivia FindEndOfLine(SyntaxToken token)
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

        public static IndentationAnalysis AnalyzeIndentation(SyntaxNode node, CancellationToken cancellationToken = default)
        {
            SyntaxTrivia indentation = DetermineIndentation(node, cancellationToken);

            int size = DetermineIndentationSize(node, cancellationToken);

            return new IndentationAnalysis(indentation, size);
        }

        public static SyntaxTrivia DetermineIndentation(SyntaxNodeOrToken nodeOrToken, CancellationToken cancellationToken = default)
        {
            SyntaxTree tree = nodeOrToken.SyntaxTree;

            if (tree == null)
                return CSharpFactory.EmptyWhitespace();

            TextSpan span = nodeOrToken.Span;

            int lineStartIndex = span.Start - tree.GetLineSpan(span, cancellationToken).StartLinePosition.Character;

            SyntaxTriviaList leading = nodeOrToken.GetLeadingTrivia();

            if (leading.Any())
            {
                SyntaxTrivia last = leading.Last();

                if (last.IsWhitespaceTrivia()
                    && lineStartIndex == span.Start - last.Span.Length)
                {
                    return last;
                }
            }

            SyntaxNode node = (nodeOrToken.IsNode)
                ? nodeOrToken.AsNode()
                : nodeOrToken.AsToken().Parent;

            while (!node.FullSpan.Contains(lineStartIndex))
                node = node.GetParent(ascendOutOfTrivia: true);

            if (node.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                if (((DocumentationCommentTriviaSyntax)node)
                    .ParentTrivia
                    .TryGetContainingList(out leading, allowTrailing: false))
                {
                    SyntaxTrivia trivia = leading.Last();

                    if (trivia.IsWhitespaceTrivia())
                        return trivia;
                }
            }
            else
            {
                SyntaxToken token = node.FindToken(lineStartIndex);

                leading = token.LeadingTrivia;

                if (leading.Any()
                    && leading.FullSpan.Contains(lineStartIndex))
                {
                    SyntaxTrivia trivia = leading.Last();

                    if (trivia.IsWhitespaceTrivia())
                        return trivia;
                }
            }

            return CSharpFactory.EmptyWhitespace();
        }

        public static int DetermineIndentationSize(SyntaxNode node, CancellationToken cancellationToken = default)
        {
            do
            {
                switch (node)
                {
                    case MemberDeclarationSyntax member:
                        {
                            switch (node.Parent)
                            {
                                case NamespaceDeclarationSyntax @namespace:
                                    {
                                        int size = GetIndentationSize(member, @namespace.CloseBraceToken);

                                        if (size > 0)
                                            return size;

                                        break;
                                    }
                                case BaseTypeDeclarationSyntax baseType:
                                    {
                                        int size = GetIndentationSize(member, baseType.CloseBraceToken);

                                        if (size > 0)
                                            return size;

                                        break;
                                    }
                                case CompilationUnitSyntax compilationUnit:
                                    {
                                        return DetermineIndentationSize(compilationUnit);
                                    }
                                default:
                                    {
                                        Debug.Fail(node.Parent.Kind().ToString());
                                        return 0;
                                    }
                            }

                            break;
                        }
                    case AccessorDeclarationSyntax accessor:
                        {
                            switch (node.Parent)
                            {
                                case AccessorListSyntax accessorList:
                                    {
                                        int size = GetIndentationSize(accessor, accessorList.CloseBraceToken);

                                        if (size > 0)
                                            return size;

                                        break;
                                    }
                                default:
                                    {
                                        Debug.Fail(node.Parent.Kind().ToString());
                                        return 0;
                                    }
                            }

                            break;
                        }
                    case StatementSyntax statement:
                        {
                            switch (node.Parent)
                            {
                                case SwitchSectionSyntax switchSection:
                                    {
                                        int size = GetIndentationSize(statement, switchSection);

                                        if (size > 0)
                                            return size;

                                        break;
                                    }
                                case BlockSyntax block:
                                    {
                                        int size = GetIndentationSize(statement, block.CloseBraceToken);

                                        if (size > 0)
                                            return size;

                                        break;
                                    }
                                default:
                                    {
                                        Debug.Fail(node.Parent.Kind().ToString());
                                        return 0;
                                    }
                            }

                            break;
                        }
                    case CompilationUnitSyntax compilationUnit:
                        {
                            return DetermineIndentationSize(compilationUnit);
                        }
                }

                node = node.Parent;

            } while (node != null);

            Debug.Fail("");

            return 0;

            int DetermineIndentationSize(CompilationUnitSyntax compilationUnit)
            {
                foreach (MemberDeclarationSyntax member in compilationUnit.Members)
                {
                    switch (member)
                    {
                        case NamespaceDeclarationSyntax namespaceDeclaration:
                            {
                                MemberDeclarationSyntax member2 = namespaceDeclaration.Members.FirstOrDefault();

                                if (member2 != null)
                                    return GetIndentationSize(member2, namespaceDeclaration.CloseBraceToken);

                                break;
                            }
                        case TypeDeclarationSyntax typeDeclaration:
                            {
                                MemberDeclarationSyntax member2 = typeDeclaration.Members.FirstOrDefault();

                                if (member2 != null)
                                    return GetIndentationSize(member2, typeDeclaration.CloseBraceToken);

                                break;
                            }
                    }
                }

                return 0;
            }

            int GetIndentationSize(SyntaxNodeOrToken nodeOrToken1, SyntaxNodeOrToken nodeOrToken2)
            {
                SyntaxTrivia indentation1 = DetermineIndentation(nodeOrToken1, cancellationToken);

                int length1 = indentation1.Span.Length;

                if (length1 > 0)
                {
                    SyntaxTrivia indentation2 = DetermineIndentation(nodeOrToken2, cancellationToken);

                    int length2 = indentation2.Span.Length;

                    Debug.Assert(length1 >= length2, $"{length1} {length2}");

                    if (length1 > length2)
                        return length1 - length2;
                }

                return 0;
            }
        }

        public static string GetIncreasedIndentation(SyntaxNode node, CancellationToken cancellationToken = default)
        {
            return AnalyzeIndentation(node, cancellationToken).GetIncreasedIndentation();
        }

        public static SyntaxTrivia GetIncreasedIndentationTrivia(SyntaxNode node, CancellationToken cancellationToken = default)
        {
            return AnalyzeIndentation(node, cancellationToken).GetIncreasedIndentationTrivia();
        }

        public static SyntaxTriviaList GetIncreasedIndentationTriviaList(SyntaxNode node, CancellationToken cancellationToken = default)
        {
            return AnalyzeIndentation(node, cancellationToken).GetIncreasedIndentationTriviaList();
        }
    }
}
