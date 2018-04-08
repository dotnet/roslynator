// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SingleLineDocumentationCommentTriviaCodeFixProvider))]
    [Shared]
    public class SingleLineDocumentationCommentTriviaCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.FormatDocumentationSummaryOnSingleLine,
                    DiagnosticIdentifiers.FormatDocumentationSummaryOnMultipleLines);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out DocumentationCommentTriviaSyntax documentationComment, findInsideTrivia: true, getInnermostNodeForTie: false))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.FormatDocumentationSummaryOnSingleLine:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Format summary on a single line",
                                cancellationToken => FormatSummaryOnSingleLineRefactoring.RefactorAsync(context.Document, documentationComment, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.FormatDocumentationSummaryOnMultipleLines:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Format summary on multiple lines",
                                cancellationToken => FormatSummaryOnMultipleLinesRefactoring.RefactorAsync(context.Document, documentationComment, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}