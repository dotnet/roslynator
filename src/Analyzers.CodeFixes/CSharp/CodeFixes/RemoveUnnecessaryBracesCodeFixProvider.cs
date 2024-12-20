// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveUnnecessaryBracesCodeFixProvider))]
[Shared]
public class RemoveUnnecessaryBracesCodeFixProvider : BaseCodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIds.RemoveUnnecessaryBraces); }
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(
            root,
            context.Span,
            out TypeDeclarationSyntax typeDeclaration))
        {
            return;
        }

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        CodeAction codeAction = CodeAction.Create(
            "Remove unnecessary braces",
            ct =>
            {
                TypeDeclarationSyntax newNode = typeDeclaration
                    .WithOpenBraceToken(default)
                    .WithCloseBraceToken(default)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

                SyntaxTriviaList trailing = typeDeclaration
                    .DescendantTrivia(TextSpan.FromBounds(
                        typeDeclaration.OpenBraceToken.GetPreviousToken().Span.End,
                        typeDeclaration.CloseBraceToken.SpanStart))
                    .ToSyntaxTriviaList()
                    .EmptyIfWhitespace()
                    .AddRange(typeDeclaration.CloseBraceToken.TrailingTrivia);

                newNode = newNode.WithTrailingTrivia(trailing);

                return document.ReplaceNodeAsync(typeDeclaration, newNode, ct);
            },
            GetEquivalenceKey(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }
}
