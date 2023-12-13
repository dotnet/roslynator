// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnnecessaryRawStringLiteralCodeFixProvider))]
[Shared]
public sealed class UnnecessaryRawStringLiteralCodeFixProvider : BaseCodeFixProvider
{
    private const string Title = "Unnecessary raw string literal";

    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.UnnecessaryRawStringLiteral); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => f.IsKind(SyntaxKind.StringLiteralExpression, SyntaxKind.InterpolatedStringExpression)))
            return;

        Diagnostic diagnostic = context.Diagnostics[0];
        Document document = context.Document;

        if (node is LiteralExpressionSyntax literalExpression)
        {
            CodeAction codeAction = CodeAction.Create(
                Title,
                ct => RefactorAsync(document, literalExpression, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
        else if (node is InterpolatedStringExpressionSyntax interpolatedString)
        {
            CodeAction codeAction = CodeAction.Create(
                Title,
                ct => RefactorAsync(document, interpolatedString, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }

    private static Task<Document> RefactorAsync(
        Document document,
        LiteralExpressionSyntax literalExpression,
        CancellationToken cancellationToken)
    {
        RawStringLiteralInfo info = RawStringLiteralInfo.Create(literalExpression);

        string newText = info.Text.Substring(info.QuoteCount - 1, info.Text.Length - ((info.QuoteCount * 2) - 2));

        return document.WithTextChangeAsync(literalExpression.Span, newText, cancellationToken);
    }

    private static Task<Document> RefactorAsync(
        Document document,
        InterpolatedStringExpressionSyntax interpolatedString,
        CancellationToken cancellationToken)
    {
        InterpolatedStringExpressionSyntax newInterpolatedString = interpolatedString.ReplaceTokens(
            interpolatedString
                .Contents
                .OfType<InterpolationSyntax>()
                .SelectMany(interpolation => new SyntaxToken[] { interpolation.OpenBraceToken, interpolation.CloseBraceToken }),
            (token, _) =>
            {
                if (token.IsKind(SyntaxKind.OpenBraceToken))
                {
                    return SyntaxFactory.Token(SyntaxKind.OpenBraceToken).WithTriviaFrom(token);
                }
                else
                {
                    return SyntaxFactory.Token(SyntaxKind.CloseBraceToken).WithTriviaFrom(token);
                }
            });

        string text = newInterpolatedString.ToString();
        int startIndex = newInterpolatedString.StringStartToken.Text.Length;
        string newText = "$\"" + text.Substring(startIndex, text.Length - startIndex - newInterpolatedString.StringEndToken.Text.Length) + "\"";

        return document.WithTextChangeAsync(interpolatedString.Span, newText, cancellationToken);
    }
}
