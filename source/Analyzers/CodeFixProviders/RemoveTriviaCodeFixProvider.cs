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

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveTriviaCodeFixProvider))]
    [Shared]
    public class RemoveTriviaCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.RemoveTrailingWhitespace,
                    DiagnosticIdentifiers.RemoveRedundantEmptyLine);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            SyntaxTrivia trivia = root.FindTrivia(context.Span.Start);

            if (trivia.IsKind(SyntaxKind.None))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.RemoveTrailingWhitespace:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove trailing white-space",
                                cancellationToken => RemoveTriviaAsync(context.Document, context.Span, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantEmptyLine:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove empty line",
                                cancellationToken => RemoveTriviaAsync(context.Document, context.Span, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static async Task<Document> RemoveTriviaAsync(
            Document document,
            TextSpan span,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var rewriter = new RemoveTriviaSyntaxRewriter(span);

            SyntaxNode newRoot = rewriter.Visit(oldRoot);

            return document.WithSyntaxRoot(newRoot);
        }

        private class RemoveTriviaSyntaxRewriter : CSharpSyntaxRewriter
        {
            private readonly TextSpan _textSpan;

            public RemoveTriviaSyntaxRewriter(TextSpan textSpan)
            {
                _textSpan = textSpan;
            }

            public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
            {
                if (_textSpan.Contains(trivia.Span))
                    return SyntaxFactory.Whitespace(string.Empty);

                return base.VisitTrivia(trivia);
            }
        }
    }
}
