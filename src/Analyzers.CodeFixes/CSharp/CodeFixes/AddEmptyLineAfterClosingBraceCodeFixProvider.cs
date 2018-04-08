// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddEmptyLineAfterClosingBraceCodeFixProvider))]
    [Shared]
    public class AddEmptyLineAfterClosingBraceCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddEmptyLineAfterClosingBrace); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindTrivia(root, context.Span.Start, out SyntaxTrivia trivia))
                return;

            Debug.Assert(trivia.Kind() == SyntaxKind.EndOfLineTrivia, trivia.Kind().ToString());

            if (trivia.Kind() != SyntaxKind.EndOfLineTrivia)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Add empty line",
                cancellationToken => RefactorAsync(context.Document, trivia.Token, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.AddEmptyLineAfterClosingBrace));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            SyntaxToken token,
            CancellationToken cancellationToken)
        {
            SyntaxToken newToken = token.AppendToTrailingTrivia(CSharpFactory.NewLine());

            return document.ReplaceTokenAsync(token, newToken, cancellationToken);
        }
    }
}
