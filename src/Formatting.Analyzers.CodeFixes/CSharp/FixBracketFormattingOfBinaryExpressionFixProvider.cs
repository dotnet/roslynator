// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FixBracketFormattingOfBinaryExpressionFixProvider))]
[Shared]
public sealed class FixBracketFormattingOfBinaryExpressionFixProvider : BaseCodeFixProvider
{
    private const string Title = "Fix bracket formatting";

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (
            !TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                predicate: f =>
                {
                    switch (f.Kind())
                    {
                        case SyntaxKind.ParenthesizedExpression:
                        case SyntaxKind.IfStatement:
                        case SyntaxKind.WhileStatement:
                        case SyntaxKind.DoStatement:
                            return true;
                        default:
                            return false;
                    }
                }
            )
        )
        {
            return;
        }

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];
        CodeAction codeAction = CreateCodeAction();

        context.RegisterCodeFix(codeAction, diagnostic);

        CodeAction CreateCodeAction()
        {
            switch (node)
            {
                case ParenthesizedExpressionSyntax parenthesizedExpression:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(parenthesizedExpression, parenthesizedExpression.OpenParenToken, parenthesizedExpression.CloseParenToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case IfStatementSyntax ifStatement:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(ifStatement, ifStatement.OpenParenToken, ifStatement.CloseParenToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case WhileStatementSyntax whileStatement:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(whileStatement, whileStatement.OpenParenToken, whileStatement.CloseParenToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case DoStatementSyntax doStatement:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(doStatement, doStatement.OpenParenToken, doStatement.CloseParenToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                default:
                {
                    throw new InvalidOperationException();
                }
            }
        }

        async Task<Document> Fix(
            SyntaxNode targetNode,
            SyntaxToken openNodeOrToken,
            SyntaxToken closeNodeOrToken,
            CancellationToken cancellationToken
        )
        {
            AnalyzerConfigOptions configOptions = document.GetConfigOptions(targetNode.SyntaxTree);
            TargetBracesStyle bracesStyle = configOptions.GetTargetBracesStyle();

            if (bracesStyle == TargetBracesStyle.None)
            {
                return document;
            }

            string endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(node).ToString();

            List<TextChange> textChanges = new ();

            if (bracesStyle.HasFlag(TargetBracesStyle.Opening))
            {
                SyntaxToken nextToken = openNodeOrToken.GetNextToken();

                int bracketLine = openNodeOrToken.GetSpanStartLine(cancellationToken);
                int nextTokenLine = nextToken.GetSpanStartLine(cancellationToken);

                if (bracketLine == nextTokenLine)
                {
                    string indentation = SyntaxTriviaAnalysis.GetIncreasedIndentation(targetNode, configOptions, cancellationToken);

                    textChanges.Add(
                        new TextChange(
                            TextSpan.FromBounds(openNodeOrToken.Span.End, nextToken.SpanStart),
                            endOfLine + indentation
                        )
                    );
                }
            }

            if (bracesStyle.HasFlag(TargetBracesStyle.Closing))
            {
                SyntaxToken previousToken = closeNodeOrToken.GetPreviousToken();

                int bracketLine = closeNodeOrToken.GetSpanStartLine(cancellationToken);
                int previousTokenLine = previousToken.GetSpanEndLine(cancellationToken);

                if (bracketLine == previousTokenLine)
                {
                    string indentation = SyntaxTriviaAnalysis.DetermineIndentation(targetNode, searchInAccessors: false, cancellationToken).ToString();

                    textChanges.Add(
                        new TextChange(
                            closeNodeOrToken.Span,
                            endOfLine + indentation + closeNodeOrToken
                        )
                    );
                }
                else
                {
                    SyntaxTrivia listNodeIndent = SyntaxTriviaAnalysis.DetermineIndentation(targetNode, searchInAccessors: false, cancellationToken);
                    SyntaxTrivia bracketIndent = SyntaxTriviaAnalysis.DetermineIndentation(closeNodeOrToken, searchInAccessors: false, cancellationToken);
                    if (listNodeIndent.Span.Length != bracketIndent.Span.Length)
                    {
                        TextSpan span =
                            bracketIndent.Span.Length == 0 // there is no indentation
                                ? new TextSpan(closeNodeOrToken.Span.Start, 0)
                                : bracketIndent.Span;

                        textChanges.Add(
                            new TextChange(
                                span,
                                listNodeIndent.ToString()
                            )
                        );
                    }
                }
            }

            return await document.WithTextChangesAsync(textChanges, cancellationToken);
        }
    }
}