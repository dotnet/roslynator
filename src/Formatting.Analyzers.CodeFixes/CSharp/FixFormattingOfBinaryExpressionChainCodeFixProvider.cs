// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;
using Roslynator.Text;
using static Roslynator.CSharp.SyntaxTriviaAnalysis;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FixFormattingOfBinaryExpressionChainCodeFixProvider))]
    [Shared]
    internal class FixFormattingOfBinaryExpressionChainCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.FixFormattingOfBinaryExpressionChain); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out BinaryExpressionSyntax binaryExpression))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            CodeAction codeAction = CodeAction.Create(
                "Fix formatting",
                ct => FixAsync(document, binaryExpression, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static Task<Document> FixAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            IndentationAnalysis indentationAnalysis = AnalyzeIndentation(binaryExpression, cancellationToken);

            string indentation;
            if (indentationAnalysis.Indentation == binaryExpression.GetLeadingTrivia().LastOrDefault()
                && document.IsAnalyzerOptionEnabled(AnalyzerOptions.AddNewLineAfterBinaryOperatorInsteadOfBeforeIt))
            {
                indentation = indentationAnalysis.Indentation.ToString();
            }
            else
            {
                indentation = indentationAnalysis.GetIncreasedIndentation();
            }

            string endOfLineAndIndentation = DetermineEndOfLine(binaryExpression).ToString() + indentation;

            var textChanges = new List<TextChange>();
            int prevIndex = binaryExpression.Span.End;

            SyntaxKind binaryKind = binaryExpression.Kind();

            while (true)
            {
                ExpressionSyntax left = binaryExpression.Left;
                SyntaxToken token = binaryExpression.OperatorToken;
                ExpressionSyntax right = binaryExpression.Right;

                SyntaxTriviaList leftTrailing = left.GetTrailingTrivia();
                SyntaxTriviaList tokenTrailing = token.TrailingTrivia;

                if (IsOptionalWhitespaceThenOptionalSingleLineCommentThenEndOfLineTrivia(leftTrailing))
                {
                    if (!SetIndentation(token))
                        break;
                }
                else if (IsOptionalWhitespaceThenOptionalSingleLineCommentThenEndOfLineTrivia(tokenTrailing))
                {
                    if (!SetIndentation(right))
                        break;
                }
                else if (leftTrailing.IsEmptyOrWhitespace()
                    && tokenTrailing.IsEmptyOrWhitespace())
                {
                    if (document.IsAnalyzerOptionEnabled(AnalyzerOptions.AddNewLineAfterBinaryOperatorInsteadOfBeforeIt))
                    {
                        if (!SetIndentation(right))
                            break;
                    }
                    else if (!SetIndentation(token))
                    {
                        break;
                    }
                }

                if (!left.IsKind(binaryKind))
                    break;

                binaryExpression = (BinaryExpressionSyntax)left;
            }

            if (textChanges.Count > 0)
            {
                SyntaxTriviaList leading = binaryExpression.GetLeadingTrivia();

                if (!leading.Any())
                {
                    SyntaxTrivia trivia = binaryExpression.GetFirstToken().GetPreviousToken().TrailingTrivia.LastOrDefault();

                    if (trivia.IsEndOfLineTrivia()
                        && trivia.Span.End == binaryExpression.SpanStart)
                    {
                        textChanges.Add(new TextSpan(binaryExpression.SpanStart, 0), indentation);
                    }
                }
            }

            FormattingVerifier.VerifyChangedSpansAreWhitespace(binaryExpression, textChanges);

            return document.WithTextChangesAsync(textChanges, cancellationToken);

            bool SetIndentation(SyntaxNodeOrToken nodeOrToken)
            {
                SyntaxTriviaList leading = nodeOrToken.GetLeadingTrivia();
                SyntaxTriviaList.Reversed.Enumerator en = leading.Reverse().GetEnumerator();

                if (!en.MoveNext())
                {
                    SyntaxTrivia trivia = binaryExpression.FindTrivia(nodeOrToken.SpanStart - 1);

                    string newText = (trivia.IsEndOfLineTrivia()) ? indentation : endOfLineAndIndentation;

                    int start = (trivia.IsWhitespaceTrivia()) ? trivia.SpanStart : nodeOrToken.SpanStart;

                    TextSpan span = (trivia.IsWhitespaceTrivia())
                        ? trivia.Span
                        : new TextSpan(nodeOrToken.SpanStart, 0);

                    textChanges.Add(span, newText);
                    SetIndendation(nodeOrToken, prevIndex);
                    prevIndex = start;
                    return true;
                }

                SyntaxTrivia last = en.Current;

                SyntaxKind kind = en.Current.Kind();

                if (kind == SyntaxKind.WhitespaceTrivia)
                {
                    if (en.Current.Span.Length != indentation.Length)
                    {
                        if (!en.MoveNext()
                            || en.Current.IsEndOfLineTrivia())
                        {
                            SyntaxTrivia trivia = binaryExpression.FindTrivia(nodeOrToken.FullSpan.Start - 1);

                            if (trivia.IsEndOfLineTrivia())
                            {
                                AddTextChange((leading.IsEmptyOrWhitespace()) ? leading.Span : last.Span);
                                SetIndendation(nodeOrToken, prevIndex);
                                prevIndex = trivia.SpanStart;
                                return true;
                            }
                        }
                    }
                }
                else if (kind == SyntaxKind.EndOfLineTrivia)
                {
                    SyntaxTrivia trivia = binaryExpression.FindTrivia(nodeOrToken.FullSpan.Start - 1);

                    if (trivia.IsEndOfLineTrivia())
                    {
                        AddTextChange((leading.IsEmptyOrWhitespace()) ? leading.Span : last.Span);
                        SetIndendation(nodeOrToken, prevIndex);
                        prevIndex = trivia.SpanStart;
                        return true;
                    }
                }

                prevIndex = leading.Span.Start - 1;
                return true;

                void AddTextChange(TextSpan span) => textChanges.Add(span, indentation);
            }

            void SetIndendation(SyntaxNodeOrToken nodeOrToken, int endIndex)
            {
                ImmutableArray<IndentationInfo> indentations = FindIndentations(binaryExpression, TextSpan.FromBounds(nodeOrToken.SpanStart, endIndex)).ToImmutableArray();

                if (!indentations.Any())
                    return;

                int firstIndentationLength = indentations[0].Span.Length;

                for (int j = 0; j < indentations.Length; j++)
                {
                    IndentationInfo indentationInfo = indentations[j];

                    string replacement = indentation + indentationAnalysis.GetSingleIndentation();

                    if (j > 0
                        && indentationInfo.Span.Length > firstIndentationLength)
                    {
                        replacement += indentationInfo.ToString().Substring(firstIndentationLength);
                    }

                    if (indentationInfo.Span.Length != replacement.Length)
                        textChanges.Add(indentationInfo.Span, replacement);
                }
            }
        }
    }
}
