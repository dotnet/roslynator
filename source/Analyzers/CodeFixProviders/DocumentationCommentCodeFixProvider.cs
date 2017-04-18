// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DocumentationCommentCodeFixProvider))]
    [Shared]
    public class DocumentationCommentCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddSummaryElementToDocumentationComment); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            DocumentationCommentTriviaSyntax documentationComment = root
                .FindNode(context.Span, findInsideTrivia: true, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<DocumentationCommentTriviaSyntax>();

            Debug.Assert(documentationComment != null, $"{nameof(documentationComment)} is null");

            if (documentationComment == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.AddSummaryElementToDocumentationComment:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Add summary element",
                                cancellationToken => AddSummaryToDocumentationCommentRefactoring.RefactorAsync(context.Document, documentationComment, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
