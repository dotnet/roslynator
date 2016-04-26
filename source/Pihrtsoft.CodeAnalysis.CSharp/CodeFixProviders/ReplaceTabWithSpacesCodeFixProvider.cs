// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
#if DEBUG
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveTriviaCodeFixProvider))]
    [Shared]
    public class ReplaceTabWithSpacesCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.UseSpacesInsteadOfTab);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            SyntaxTrivia trivia = root.FindTrivia(context.Span.Start);

            if (!trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                return;

            CodeAction codeAction = CodeAction.Create(
                "Replace tab with spaces",
                cancellationToken => ReplaceTabWithSpacesAsync(context.Document, context.Span, cancellationToken),
                DiagnosticIdentifiers.UseSpacesInsteadOfTab + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> ReplaceTabWithSpacesAsync(
            Document document,
            TextSpan span,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var rewriter = new ReplaceTabWithSpacesSyntaxRewriter(span);

            SyntaxNode newRoot = rewriter.Visit(oldRoot);

            return document.WithSyntaxRoot(newRoot);
        }

        private class ReplaceTabWithSpacesSyntaxRewriter : CSharpSyntaxRewriter
        {
            private readonly TextSpan _textSpan;

            public ReplaceTabWithSpacesSyntaxRewriter(TextSpan textSpan)
            {
                _textSpan = textSpan;
            }

            public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
            {
                if (_textSpan.Contains(trivia.Span))
                    return SyntaxFactory.Whitespace(trivia.ToString().Replace("\t", "    "));

                return base.VisitTrivia(trivia);
            }
        }
    }
#endif
}
