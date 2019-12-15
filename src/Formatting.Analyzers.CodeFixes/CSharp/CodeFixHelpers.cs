// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;
using Roslynator.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    internal static class CodeFixHelpers
    {
        public static Task<Document> AppendEndOfLineAsync(
            Document document,
            SyntaxToken token,
            CancellationToken cancellationToken = default)
        {
            SyntaxToken newToken = token.AppendEndOfLineToTrailingTrivia();

            return document.ReplaceTokenAsync(token, newToken, cancellationToken);
        }

        public static Task<Document> AppendEndOfLineAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken = default)
        {
            SyntaxNode newNode = node.AppendEndOfLineToTrailingTrivia();

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        public static Task<Document> AddNewLineBeforeAsync(
            Document document,
            SyntaxToken token,
            CancellationToken cancellationToken = default)
        {
            SyntaxTrivia indentation = token.Parent.GetIndentation(cancellationToken);

            return AddNewLineBeforeAsync(
                document,
                token,
                indentation.ToString(),
                cancellationToken);
        }

        public static Task<Document> AddNewLineBeforeAndIncreaseIndentationAsync(
            Document document,
            SyntaxToken token,
            CancellationToken cancellationToken = default)
        {
            return AddNewLineBeforeAndIncreaseIndentationAsync(
                document,
                token,
                token.Parent.GetIndentation(cancellationToken),
                cancellationToken);
        }

        public static Task<Document> AddNewLineBeforeAndIncreaseIndentationAsync(
            Document document,
            SyntaxToken token,
            SyntaxTrivia indentation,
            CancellationToken cancellationToken = default)
        {
            SyntaxTrivia singleIndentation = token.SyntaxTree.GetFirstIndentation(cancellationToken);

            return AddNewLineBeforeAsync(
                document,
                token,
                indentation.ToString() + singleIndentation.ToString(),
                cancellationToken);
        }

        public static Task<Document> AddNewLineBeforeAsync(
            Document document,
            SyntaxToken token,
            string indentation,
            CancellationToken cancellationToken = default)
        {
            var textChange = new TextChange(
                TextSpan.FromBounds(token.GetPreviousToken().Span.End, token.SpanStart),
                SyntaxTriviaAnalysis.GetEndOfLine(token).ToString() + indentation);

            return document.WithTextChangeAsync(textChange, cancellationToken);
        }

        public static (ExpressionSyntax left, SyntaxToken token, ExpressionSyntax right) AddNewLineBeforeTokenInsteadOfAfterIt(
            ExpressionSyntax left,
            SyntaxToken token,
            ExpressionSyntax right)
        {
            return (
                left.WithTrailingTrivia(token.TrailingTrivia),
                Token(
                    right.GetLeadingTrivia(),
                    token.Kind(),
                    TriviaList(Space)),
                right.WithoutLeadingTrivia());
        }

        public static (ExpressionSyntax left, SyntaxToken token, ExpressionSyntax right) AddNewLineAfterTokenInsteadOfBeforeIt(
            ExpressionSyntax left,
            SyntaxToken token,
            ExpressionSyntax right)
        {
            return (
                left.WithTrailingTrivia(Space),
                Token(
                    SyntaxTriviaList.Empty,
                    token.Kind(),
                    left.GetTrailingTrivia()),
                right.WithLeadingTrivia(token.LeadingTrivia));
        }

        public static Task<Document> AddEmptyLineBeforeDirectiveAsync(
            Document document,
            DirectiveTriviaSyntax directiveTrivia,
            CancellationToken cancellationToken = default)
        {
            SyntaxTrivia parentTrivia = directiveTrivia.ParentTrivia;
            SyntaxToken token = parentTrivia.Token;
            SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

            int index = leadingTrivia.IndexOf(parentTrivia);

            if (index > 0
                && leadingTrivia[index - 1].IsWhitespaceTrivia())
            {
                index--;
            }

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(index, SyntaxTriviaAnalysis.GetEndOfLine(token));

            SyntaxToken newToken = token.WithLeadingTrivia(newLeadingTrivia);

            return document.ReplaceTokenAsync(token, newToken, cancellationToken);
        }

        public static Task<Document> AddEmptyLineAfterDirectiveAsync(
            Document document,
            DirectiveTriviaSyntax directiveTrivia,
            CancellationToken cancellationToken = default)
        {
            SyntaxTrivia parentTrivia = directiveTrivia.ParentTrivia;
            SyntaxToken token = parentTrivia.Token;
            SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

            int index = leadingTrivia.IndexOf(parentTrivia);

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(index + 1, SyntaxTriviaAnalysis.GetEndOfLine(token));

            SyntaxToken newToken = token.WithLeadingTrivia(newLeadingTrivia);

            return document.ReplaceTokenAsync(token, newToken, cancellationToken);
        }

        public static Task<Document> AddNewLineAfterInsteadOfBeforeAsync(
            Document document,
            SyntaxNodeOrToken left,
            SyntaxNodeOrToken middle,
            SyntaxNodeOrToken right,
            CancellationToken cancellationToken = default)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            sb.Append(" ");
            sb.Append(middle.ToString());

            SyntaxTriviaList trailingTrivia = left.GetTrailingTrivia();

            if (SyntaxTriviaAnalysis.IsOptionalWhitespaceThenEndOfLineTrivia(trailingTrivia))
            {
                sb.Append(SyntaxTriviaAnalysis.GetEndOfLine(left).ToString());
            }
            else
            {
                sb.Append(trailingTrivia.ToString());
            }

            sb.Append(middle.GetLeadingTrivia().ToString());

            string newText = StringBuilderCache.GetStringAndFree(sb);

            var textChange = new TextChange(
                TextSpan.FromBounds(left.Span.End, right.SpanStart),
                newText);

            return document.WithTextChangeAsync(textChange, cancellationToken);
        }

        public static Task<Document> AddNewLineBeforeInsteadOfAfterAsync(
            Document document,
            SyntaxNodeOrToken left,
            SyntaxNodeOrToken middle,
            SyntaxNodeOrToken right,
            CancellationToken cancellationToken = default)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            SyntaxTriviaList trailingTrivia = middle.GetTrailingTrivia();

            if (SyntaxTriviaAnalysis.IsOptionalWhitespaceThenEndOfLineTrivia(trailingTrivia))
            {
                sb.Append(SyntaxTriviaAnalysis.GetEndOfLine(middle).ToString());
            }
            else
            {
                sb.Append(trailingTrivia.ToString());
            }

            sb.Append(right.GetLeadingTrivia().ToString());
            sb.Append(middle.ToString());
            sb.Append(" ");

            string newText = StringBuilderCache.GetStringAndFree(sb);

            var textChange = new TextChange(
                TextSpan.FromBounds(left.Span.End, right.SpanStart),
                newText);

            return document.WithTextChangeAsync(textChange, cancellationToken);
        }

        public static Task<Document> RemoveEmptyLinesBeforeAsync(
            Document document,
            SyntaxToken token,
            CancellationToken cancellationToken = default)
        {
            SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

            int count = 0;

            SyntaxTriviaList.Enumerator en = leadingTrivia.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current.IsWhitespaceTrivia())
                {
                    if (!en.MoveNext())
                        break;

                    if (!en.Current.IsEndOfLineTrivia())
                        break;

                    count += 2;
                }
                else if (en.Current.IsEndOfLineTrivia())
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            SyntaxToken newToken = token.WithLeadingTrivia(leadingTrivia.RemoveRange(0, count));

            return document.ReplaceTokenAsync(token, newToken, cancellationToken);
        }

        public static Task<Document> AddNewLineAfterOpeningBraceAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken = default)
        {
            BlockSyntax newBlock = block
                .WithOpenBraceToken(block.OpenBraceToken.AppendEndOfLineToTrailingTrivia())
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }

        public static Task<Document> ReplaceTriviaBetweenAsync(
            Document document,
            SyntaxToken token1,
            SyntaxToken token2,
            string replacement = " ",
            CancellationToken cancellationToken = default)
        {
            TextSpan span = TextSpan.FromBounds(token1.Span.End, token2.SpanStart);

            var textChange = new TextChange(span, replacement);

            return document.WithTextChangeAsync(textChange, cancellationToken);
        }
    }
}
