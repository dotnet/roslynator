﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;
using Roslynator.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp;

internal static class SyntaxTriviaAnalysis
{
    public static TriviaBlockAnalysis AnalyzeBefore(SyntaxNodeOrToken nodeOrToken)
    {
        var en = new TriviaBlockAnalysis.Enumerator(default, nodeOrToken);

        return Analyze(ref en);
    }

    public static TriviaBlockAnalysis AnalyzeAfter(SyntaxNodeOrToken nodeOrToken)
    {
        var en = new TriviaBlockAnalysis.Enumerator(nodeOrToken, default);

        return Analyze(ref en);
    }

    public static TriviaBlockAnalysis AnalyzeBetween(SyntaxNodeOrToken first, SyntaxNodeOrToken second)
    {
        Debug.Assert(first.FullSpan.End == second.FullSpan.Start, $"{first.FullSpan.End} {second.FullSpan.Start}");

        var en = new TriviaBlockAnalysis.Enumerator(first, second);

        return Analyze(ref en);
    }

    private static TriviaBlockAnalysis Analyze(ref TriviaBlockAnalysis.Enumerator en)
    {
        var flags = TriviaBlockFlags.None;

        if (!en.MoveNext())
            return en.GetResult(TriviaBlockKind.NoNewLine, flags);

        if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
            return en.GetResult(TriviaBlockKind.NoNewLine, flags);

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
        {
            flags |= TriviaBlockFlags.SingleLineComment;

            if (!en.MoveNext())
                return default;
        }

        if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            return en.GetResult(TriviaBlockKind.NoNewLine, flags);

        if (!en.Current.IsEndOfLineTrivia())
            return default;

        if (!en.MoveNext())
            return en.GetResult(TriviaBlockKind.NewLine, flags);

        if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
            return en.GetResult(TriviaBlockKind.NewLine, flags);

        if (en.Current.IsDirective)
            return default;

        var singleLineComment = false;

        if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
        {
            flags |= TriviaBlockFlags.SingleLineComment;
            singleLineComment = true;

            if (!en.MoveNext())
                return default;

            if (!en.Current.IsEndOfLineTrivia())
                return default;
        }
        else if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
        {
            return en.GetResult(TriviaBlockKind.NewLine, flags | TriviaBlockFlags.DocumentationComment);
        }
        else if (!en.Current.IsEndOfLineTrivia())
        {
            return default;
        }

        TriviaBlockKind kind = (singleLineComment)
            ? TriviaBlockKind.NewLine
            : TriviaBlockKind.BlankLine;

        while (true)
        {
            if (!en.MoveNext())
                return en.GetResult(kind, flags);

            if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
                return en.GetResult(kind, flags);

            if (en.Current.IsDirective)
                return default;

            if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
            {
                flags |= TriviaBlockFlags.SingleLineComment;
                singleLineComment = true;

                if (!en.MoveNext())
                    return default;

                if (!en.Current.IsEndOfLineTrivia())
                    return default;
            }
            else if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia, SyntaxKind.MultiLineDocumentationCommentTrivia))
            {
                return en.GetResult(kind, flags | TriviaBlockFlags.DocumentationComment);
            }
            else if (en.Current.IsEndOfLineTrivia())
            {
                if (singleLineComment)
                    return default;
            }
            else
            {
                return default;
            }
        }
    }

    public static TriviaBlockAnalysis AnalyzeAround(
        SyntaxToken token,
        ExpressionSyntax nextExpression,
        NewLinePosition newLinePosition)
    {
        if (newLinePosition == NewLinePosition.None)
            return default;

        SyntaxToken previousToken = token.GetPreviousToken();

        TriviaBlockAnalysis analysisAfter = AnalyzeBetween(token, nextExpression);

        if (!analysisAfter.Success)
            return default;

        TriviaBlockAnalysis analysisBefore = AnalyzeBetween(previousToken, token);

        if (!analysisBefore.Success)
            return default;

        if (analysisBefore.Kind == TriviaBlockKind.NoNewLine)
        {
            if (analysisAfter.Kind == TriviaBlockKind.NoNewLine)
                return default;

            return (newLinePosition == NewLinePosition.Before)
                ? analysisAfter
                : default;
        }

        if (analysisBefore.ContainsSingleLineComment)
            return default;

        return (newLinePosition == NewLinePosition.Before)
            ? default
            : analysisBefore;
    }

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
        }
        while (!t.IsKind(SyntaxKind.None));

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

    public static IndentationAnalysis AnalyzeIndentation(SyntaxNode node, AnalyzerConfigOptions configOptions, CancellationToken cancellationToken = default)
    {
        return IndentationAnalysis.Create(node, configOptions, cancellationToken);
    }

    public static SyntaxTrivia DetermineIndentation(SyntaxNodeOrToken nodeOrToken, CancellationToken cancellationToken = default)
    {
        SyntaxTree tree = nodeOrToken.SyntaxTree;

        if (tree is null)
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

            while (node is not null)
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

    public static string GetIncreasedIndentation(SyntaxNode node, AnalyzerConfigOptions configOptions, CancellationToken cancellationToken = default)
    {
        return AnalyzeIndentation(node, configOptions, cancellationToken).GetIncreasedIndentation();
    }

    public static SyntaxTrivia GetIncreasedIndentationTrivia(SyntaxNode node, AnalyzerConfigOptions configOptions, CancellationToken cancellationToken = default)
    {
        return AnalyzeIndentation(node, configOptions, cancellationToken).GetIncreasedIndentationTrivia();
    }

    public static SyntaxTriviaList GetIncreasedIndentationTriviaList(SyntaxNode node, AnalyzerConfigOptions configOptions, CancellationToken cancellationToken = default)
    {
        return AnalyzeIndentation(node, configOptions, cancellationToken).GetIncreasedIndentationTriviaList();
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
        AnalyzerConfigOptions configOptions,
        int increaseCount = 0) where TNode : SyntaxNode
    {
        IndentationAnalysis analysis = AnalyzeIndentation(containingDeclaration, configOptions);

        string replacement = (increaseCount > 0)
            ? string.Concat(Enumerable.Repeat(analysis.GetSingleIndentation(), increaseCount))
            : "";

        replacement = analysis.GetIncreasedIndentation() + replacement;

        var builder = new SyntaxNodeTextBuilder(expression);
        int length = -1;
        int pos = expression.FullSpan.Start;

        foreach (IndentationInfo indentation in FindIndentations(expression, expression.Span)
            .OrderBy(f => f.Span.Start))
        {
            if (length == -1)
                length = indentation.Span.Length;

            builder.Append(TextSpan.FromBounds(pos, indentation.Span.Start));

            if (indentation.Span.Length == 0)
            {
                builder.Append(replacement);
            }
            else
            {
                string newIndentation = (indentation.Span.Length > length)
                    ? replacement + indentation.ToString().Substring(length)
                    : replacement;

                builder.Append(newIndentation);
            }

            pos = indentation.Span.End;
        }

        builder.Append(TextSpan.FromBounds(pos, expression.FullSpan.End));

        return (TNode)(SyntaxNode)ParseExpression(builder.ToString());
    }
}
