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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FixFormattingOfCallChainCodeFixProvider))]
    [Shared]
    internal class FixFormattingOfCallChainCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.FixFormattingOfCallChain); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out ExpressionSyntax expression,
                predicate: f => f.IsKind(
                    SyntaxKind.InvocationExpression,
                    SyntaxKind.ElementAccessExpression,
                    SyntaxKind.ConditionalAccessExpression)))
            {
                return;
            }

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            CodeAction codeAction = CodeAction.Create(
                "Fix formatting",
                ct => FixAsync(document, expression, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static Task<Document> FixAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            IndentationAnalysis indentationAnalysis = AnalyzeIndentation(expression, cancellationToken);
            string indentation = indentationAnalysis.GetIncreasedIndentation();
            string endOfLineAndIndentation = DetermineEndOfLine(expression).ToString() + indentation;

            var textChanges = new List<TextChange>();
            TextLineCollection lines = expression.SyntaxTree.GetText().Lines;
            int startLine = lines.IndexOf(expression.SpanStart);
            int prevIndex = expression.Span.End;

            foreach (SyntaxNode node in new MethodChain(expression))
            {
                SyntaxKind kind = node.Kind();

                if (kind == SyntaxKind.SimpleMemberAccessExpression)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)node;

                    if (!SetIndentation(memberAccess.OperatorToken))
                        break;
                }
                else if (kind == SyntaxKind.MemberBindingExpression)
                {
                    var memberBinding = (MemberBindingExpressionSyntax)node;

                    if (!SetIndentation(memberBinding.OperatorToken))
                        break;
                }
            }

            FormattingVerifier.VerifyChangedSpansAreWhitespace(expression, textChanges);

            return document.WithTextChangesAsync(textChanges, cancellationToken);

            bool SetIndentation(SyntaxToken token)
            {
                SyntaxTriviaList leading = token.LeadingTrivia;
                SyntaxTriviaList.Reversed.Enumerator en = leading.Reverse().GetEnumerator();

                if (!en.MoveNext())
                {
                    int endLine = lines.IndexOf(token.SpanStart);

                    if (startLine == endLine)
                        return false;

                    SyntaxTrivia trivia = expression.FindTrivia(token.SpanStart - 1);

                    string newText = (trivia.IsEndOfLineTrivia()) ? indentation : endOfLineAndIndentation;

                    textChanges.Add(new TextSpan(token.SpanStart, 0), newText);

                    SetIndendation(token, prevIndex);
                    prevIndex = (trivia.IsEndOfLineTrivia()) ? trivia.SpanStart : token.SpanStart;
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
                            SyntaxTrivia trivia = expression.FindTrivia(token.FullSpan.Start - 1);

                            if (trivia.IsEndOfLineTrivia())
                            {
                                textChanges.Add((leading.IsEmptyOrWhitespace()) ? leading.Span : last.Span, indentation);
                                SetIndendation(token, prevIndex);
                                prevIndex = trivia.SpanStart;
                                return true;
                            }
                        }
                    }
                }
                else if (kind == SyntaxKind.EndOfLineTrivia)
                {
                    SyntaxTrivia trivia = expression.FindTrivia(token.FullSpan.Start - 1);

                    if (trivia.IsEndOfLineTrivia())
                    {
                        textChanges.Add((leading.IsEmptyOrWhitespace()) ? leading.Span : last.Span, indentation);
                        SetIndendation(token, prevIndex);
                        prevIndex = trivia.SpanStart;
                        return true;
                    }
                }

                prevIndex = leading.Span.Start - 1;
                return true;
            }

            void SetIndendation(SyntaxToken token, int endIndex)
            {
                ImmutableArray<IndentationInfo> indentations = FindIndentations(expression, TextSpan.FromBounds(token.SpanStart, endIndex)).ToImmutableArray();

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
