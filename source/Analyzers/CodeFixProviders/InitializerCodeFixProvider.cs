// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InitializerCodeFixProvider))]
    [Shared]
    public class InitializerCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.RemoveRedundantCommaInInitializer);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            InitializerExpressionSyntax initializer = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InitializerExpressionSyntax>();

            if (initializer == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove redundant comma",
                cancellationToken => RemoveRedundantCommaAsync(context.Document, initializer, cancellationToken),
                DiagnosticIdentifiers.RemoveRedundantCommaInInitializer + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> RemoveRedundantCommaAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxToken lastComma = initializer.Expressions.GetSeparators().Last();

            SyntaxTriviaList newTrailingTrivia = initializer.Expressions.Last().GetTrailingTrivia()
                .AddRange(lastComma.LeadingTrivia)
                .AddRange(lastComma.TrailingTrivia);

            SeparatedSyntaxList<ExpressionSyntax> newExpressions = initializer
                .Expressions
                .ReplaceSeparator(
                    lastComma,
                    SyntaxFactory.MissingToken(SyntaxKind.CommaToken));

            ExpressionSyntax lastExpression = newExpressions.Last();

            newExpressions = newExpressions
                .Replace(lastExpression, lastExpression.WithTrailingTrivia(newTrailingTrivia));

            InitializerExpressionSyntax newInitializer = initializer
                .WithExpressions(newExpressions);

            SyntaxNode newRoot = oldRoot.ReplaceNode(initializer, newInitializer);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
