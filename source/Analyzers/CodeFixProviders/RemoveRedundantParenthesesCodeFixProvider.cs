// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveRedundantParenthesesCodeFixProvider))]
    [Shared]
    public class RemoveRedundantParenthesesCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.RemoveRedundantParentheses);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            ParenthesizedExpressionSyntax parenthesizedExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ParenthesizedExpressionSyntax>();

            if (parenthesizedExpression == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove redundant parentheses",
                cancellationToken => RemoveRedundantParenthesesAsync(context.Document, parenthesizedExpression, cancellationToken),
                DiagnosticIdentifiers.RemoveRedundantParentheses + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> RemoveRedundantParenthesesAsync(
            Document document,
            ParenthesizedExpressionSyntax parenthesizedExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax newNode = parenthesizedExpression.Expression
                .WithTriviaFrom(parenthesizedExpression);

            SyntaxNode newRoot = oldRoot.ReplaceNode(parenthesizedExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
