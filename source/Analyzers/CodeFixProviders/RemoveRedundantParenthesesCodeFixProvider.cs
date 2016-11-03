// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveRedundantParenthesesCodeFixProvider))]
    [Shared]
    public class RemoveRedundantParenthesesCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveRedundantParentheses); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            ParenthesizedExpressionSyntax parenthesizedExpression = await context
                .FindNodeAsync<ParenthesizedExpressionSyntax>(getInnermostNodeForTie: true)
                .ConfigureAwait(false);

            CodeAction codeAction = CodeAction.Create(
                "Remove redundant parentheses",
                cancellationToken => RefactorAsync(context.Document, parenthesizedExpression, cancellationToken),
                DiagnosticIdentifiers.RemoveRedundantParentheses + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ParenthesizedExpressionSyntax parenthesizedExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax expression = parenthesizedExpression.Expression;

            IEnumerable<SyntaxTrivia> leading = parenthesizedExpression.GetLeadingTrivia()
                .Concat(parenthesizedExpression.OpenParenToken.TrailingTrivia)
                .Concat(expression.GetLeadingTrivia());

            IEnumerable<SyntaxTrivia> trailing = expression.GetTrailingTrivia()
                .Concat(parenthesizedExpression.CloseParenToken.LeadingTrivia)
                .Concat(parenthesizedExpression.GetTrailingTrivia());

            SyntaxNode newRoot = root.ReplaceNode(
                parenthesizedExpression,
                expression.WithTrivia(leading, trailing));

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
