// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CommentOutRefactoring
    {
        public static void RegisterRefactoring(RefactoringContext context, MemberDeclarationSyntax member)
        {
            FileLinePositionSpan fileSpan = GetFileLinePositionSpan(member, context.CancellationToken);

            context.RegisterRefactoring(
                $"Comment out {CSharpFacts.GetTitle(member)}",
                cancellationToken => RefactorAsync(context.Document, fileSpan.StartLine(), fileSpan.EndLine(), cancellationToken));
        }

        public static void RegisterRefactoring(RefactoringContext context, LocalFunctionStatementSyntax localFunctionStatement)
        {
            FileLinePositionSpan fileSpan = GetFileLinePositionSpan(localFunctionStatement, context.CancellationToken);

            context.RegisterRefactoring(
                $"Comment out {CSharpFacts.GetTitle(localFunctionStatement)}",
                cancellationToken => RefactorAsync(context.Document, fileSpan.StartLine(), fileSpan.EndLine(), cancellationToken));
        }

        public static void RegisterRefactoring(RefactoringContext context, StatementSyntax statement)
        {
            FileLinePositionSpan fileSpan = statement.SyntaxTree.GetLineSpan(statement.Span, context.CancellationToken);

            context.RegisterRefactoring(
                "Comment out statement",
                ct => RefactorAsync(context.Document, fileSpan.StartLine(), fileSpan.EndLine(), ct));
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            int startLine,
            int endLine,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            int minIndentLength = GetMinIndentLength(sourceText, startLine, endLine);

            if (minIndentLength >= 0)
            {
                string newText = CommentOutLines(sourceText, startLine, endLine, minIndentLength);

                TextSpan span = TextSpan.FromBounds(
                    sourceText.Lines[startLine].Span.Start,
                    sourceText.Lines[endLine].SpanIncludingLineBreak.End);

                var textChange = new TextChange(span, newText);

                SourceText newSourceText = sourceText.WithChanges(textChange);

                return document.WithText(newSourceText);
            }

            return document;
        }

        private static int GetMinIndentLength(SourceText sourceText, int startLine, int endLine)
        {
            int minIndentLength = -1;

            for (int i = startLine; i <= endLine; i++)
            {
                TextLine textLine = sourceText.Lines[i];

                int indentLength = GetIndentLength(textLine.ToString());

                if (indentLength < textLine.Span.Length)
                {
                    if (minIndentLength == -1 || indentLength < minIndentLength)
                    {
                        minIndentLength = indentLength;
                    }
                }
            }

            return minIndentLength;
        }

        private static string CommentOutLines(SourceText sourceText, int startLine, int endLine, int minIndentLength)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            for (int i = startLine; i <= endLine; i++)
            {
                TextLine textLine = sourceText.Lines[i];
                string s = textLine.ToString();

                if (StringUtility.IsWhitespace(s))
                {
                    sb.Append(s);
                }
                else
                {
                    sb.Append(s, 0, minIndentLength);
                    sb.Append("//");
                    sb.Append(s, minIndentLength, s.Length - minIndentLength);
                }

                sb.Append(sourceText.GetSubText(TextSpan.FromBounds(textLine.Span.End, textLine.SpanIncludingLineBreak.End)));
            }

            return StringBuilderCache.GetStringAndFree(sb);
        }

        private static FileLinePositionSpan GetFileLinePositionSpan(SyntaxNode node, CancellationToken cancellationToken)
        {
            SyntaxTrivia trivia = node
                .GetLeadingTrivia()
                .Reverse()
                .SkipWhile(f => f.IsWhitespaceOrEndOfLineTrivia())
                .FirstOrDefault();

            if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                TextSpan span = TextSpan.FromBounds(trivia.SpanStart, node.Span.End);

                return node.SyntaxTree.GetLineSpan(span, cancellationToken);
            }
            else
            {
                return node.SyntaxTree.GetLineSpan(node.Span, cancellationToken);
            }
        }

        private static int GetIndentLength(string value)
        {
            int i = 0;
            while (i < value.Length
                && char.IsWhiteSpace(value, i))
            {
                i++;
            }

            return i;
        }
    }
}
