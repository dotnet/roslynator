// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddOrRemoveParenthesesWhenCreatingNewObjectCodeFixProvider))]
    [Shared]
    public sealed class AddOrRemoveParenthesesWhenCreatingNewObjectCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.IncludeParenthesesWhenCreatingNewObject); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ObjectCreationExpressionSyntax objectCreationExpression))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            if (objectCreationExpression.ArgumentList != null)
            {
                CodeAction codeAction = CodeAction.Create(
                    "Remove parentheses",
                    ct => RemoveParenthesesAsync(document, objectCreationExpression.ArgumentList, ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else
            {
                CodeAction codeAction = CodeAction.Create(
                    "Add parentheses",
                    ct => AddParenthesesAsync(document, objectCreationExpression, ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }

        private static Task<Document> AddParenthesesAsync(
            Document document,
            ObjectCreationExpressionSyntax objectCreationExpression,
            CancellationToken cancellationToken)
        {
            ObjectCreationExpressionSyntax newNode = objectCreationExpression
                .WithType(objectCreationExpression.Type.WithoutTrailingTrivia())
                .WithArgumentList(
                    SyntaxFactory.ArgumentList()
                        .WithTrailingTrivia(objectCreationExpression.Type.GetTrailingTrivia()));

            return document.ReplaceNodeAsync(objectCreationExpression, newNode, cancellationToken);
        }

        private static Task<Document> RemoveParenthesesAsync(
            Document document,
            ArgumentListSyntax argumentList,
            CancellationToken cancellationToken)
        {
            return document.RemoveNodeAsync(argumentList, SyntaxRemoveOptions.KeepExteriorTrivia, cancellationToken);
        }
    }
}
