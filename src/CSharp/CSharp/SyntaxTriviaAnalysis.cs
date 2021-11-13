// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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
            return IndentationAnalysis.Create(node, cancellationToken);
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

            SyntaxNode node2 = node;

            while (!node2.FullSpan.Contains(lineStartIndex))
                node2 = node2.GetParent(ascendOutOfTrivia: true);

            if (node2.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                if (((DocumentationCommentTriviaSyntax)node2)
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
                SyntaxToken token = node2.FindToken(lineStartIndex);

                leading = token.LeadingTrivia;

                if (leading.Any()
                    && leading.FullSpan.Contains(lineStartIndex))
                {
                    SyntaxTrivia trivia = leading.Last();

                    if (trivia.IsWhitespaceTrivia())
                        return trivia;
                }
            }

            if (!IsMemberDeclarationOrStatementOrAccessorDeclaration(node))
            {
                node = node.Parent;

                while (node != null)
                {
                    if (IsMemberDeclarationOrStatementOrAccessorDeclaration(node))
                    {
                        leading = node.GetLeadingTrivia();

                        if (leading.Any())
                        {
                            SyntaxTrivia trivia = leading.Last();

                            if (trivia.IsWhitespaceTrivia())
                                return trivia;
                        }

                        break;
                    }

                    node = node.Parent;
                }
            }

            return CSharpFactory.EmptyWhitespace();

            static bool IsMemberDeclarationOrStatementOrAccessorDeclaration(SyntaxNode node)
            {
                return node is MemberDeclarationSyntax
                    || (node is StatementSyntax && !node.IsKind(SyntaxKind.Block))
                    || node is AccessorDeclarationSyntax;
            }
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
                                        return 0;
                                    }
                            }

                            break;
                        }
                    case BlockSyntax _:
                        {
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
                                case StatementSyntax statement2:
                                    {
                                        int size = GetIndentationSize(statement, statement2);

                                        if (size > 0)
                                            return size;

                                        break;
                                    }
                                case ElseClauseSyntax elseClause:
                                    {
                                        int size = GetIndentationSize(statement, elseClause);

                                        if (size > 0)
                                            return size;

                                        break;
                                    }
                                case GlobalStatementSyntax:
                                    {
                                        break;
                                    }
                                default:
                                    {
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

            return 0;

            int DetermineIndentationSize(CompilationUnitSyntax compilationUnit)
            {
                foreach (MemberDeclarationSyntax member in compilationUnit.Members)
                {
                    if (member is NamespaceDeclarationSyntax namespaceDeclaration)
                    {
                        MemberDeclarationSyntax member2 = namespaceDeclaration.Members.FirstOrDefault();

                        if (member2 != null)
                            return GetIndentationSize(member2, namespaceDeclaration.CloseBraceToken);
                    }
                    else if (member is TypeDeclarationSyntax typeDeclaration)
                    {
                        MemberDeclarationSyntax member2 = typeDeclaration.Members.FirstOrDefault();

                        if (member2 != null)
                            return GetIndentationSize(member2, typeDeclaration.CloseBraceToken);
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

        public static int GetIncreasedIndentationLength(SyntaxNode node, CancellationToken cancellationToken = default)
        {
            return AnalyzeIndentation(node, cancellationToken).IncreasedIndentationLength;
        }

        public static SyntaxTrivia GetIncreasedIndentationTrivia(SyntaxNode node, CancellationToken cancellationToken = default)
        {
            return AnalyzeIndentation(node, cancellationToken).GetIncreasedIndentationTrivia();
        }

        public static SyntaxTriviaList GetIncreasedIndentationTriviaList(SyntaxNode node, CancellationToken cancellationToken = default)
        {
            return AnalyzeIndentation(node, cancellationToken).GetIncreasedIndentationTriviaList();
        }

        public static IEnumerable<IndentationInfo> FindIndentations(SyntaxNode node)
        {
            return FindIndentations(node, node.FullSpan);
        }

        public static IEnumerable<IndentationInfo> FindIndentations(SyntaxNode node, TextSpan span)
        {
            foreach (SyntaxTrivia trivia in node.DescendantTrivia(span))
            {
                if (trivia.IsKind(SyntaxKind.EndOfLineTrivia, SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    int position = trivia.Span.End;

                    if (span.Contains(position))
                    {
                        SyntaxTrivia trivia2 = node.FindTrivia(position);

                        SyntaxKind kind = trivia2.Kind();

                        if (kind == SyntaxKind.WhitespaceTrivia)
                        {
                            if (position == trivia2.SpanStart
                                && span.Contains(trivia2.Span))
                            {
                                yield return new IndentationInfo(trivia2.Token, trivia2.Span);
                            }
                        }
                        else if (kind != SyntaxKind.EndOfLineTrivia)
                        {
                            SyntaxToken token = node.FindToken(position);

                            if (position == token.SpanStart
                                && span.Contains(token.SpanStart))
                            {
                                yield return new IndentationInfo(token, new TextSpan(token.SpanStart, 0));
                            }
                        }
                    }
                }
            }
        }

        public static TNode SetIndentation<TNode>(
            TNode expression,
            SyntaxNode containingDeclaration,
            int increaseCount = 0) where TNode : SyntaxNode
        {
            ImmutableDictionary<SyntaxToken, IndentationInfo> indentations = null;
            int length;

            using (IEnumerator<IndentationInfo> en = FindIndentations(expression, expression.Span).GetEnumerator())
            {
                if (!en.MoveNext())
                    return expression;

                ImmutableDictionary<SyntaxToken, IndentationInfo>.Builder builder = ImmutableDictionary.CreateBuilder<SyntaxToken, IndentationInfo>();
                length = en.Current.Span.Length;

                do
                {
                    builder.Add(en.Current.Token, en.Current);

                } while (en.MoveNext());

                indentations = builder.ToImmutableDictionary();
            }

            IndentationAnalysis analysis = AnalyzeIndentation(containingDeclaration);

            string increasedIndentation = analysis.GetIncreasedIndentation();

            string replacement = (increaseCount > 0)
                ? string.Concat(Enumerable.Repeat(analysis.GetSingleIndentation(), increaseCount))
                : "";

            replacement = increasedIndentation + replacement;

            SyntaxTrivia replacementTrivia = Whitespace(replacement);

            return expression.ReplaceTokens(
                indentations.Select(f => f.Key),
                (token, _) =>
                {
                    IndentationInfo indentationInfo = indentations[token];

                    SyntaxTrivia newIndentation = (indentationInfo.Span.Length > length)
                        ? Whitespace(replacement + indentationInfo.ToString().Substring(length))
                        : replacementTrivia;

                    if (indentationInfo.Span.Length == 0)
                        return token.AppendToLeadingTrivia(newIndentation);

                    SyntaxTriviaList leading = token.LeadingTrivia;

                    for (int i = leading.Count - 1; i >= 0; i--)
                    {
                        if (leading[i].Span == indentationInfo.Span)
                        {
                            SyntaxTriviaList newLeading = leading.ReplaceAt(i, newIndentation);
                            return token.WithLeadingTrivia(newLeading);
                        }
                    }

                    SyntaxDebug.Fail(token);
                    return token;
                });
        }
    }
}
