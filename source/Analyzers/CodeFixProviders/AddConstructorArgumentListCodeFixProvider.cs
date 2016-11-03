// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddConstructorArgumentListCodeFixProvider))]
    [Shared]
    public class AddConstructorArgumentListCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.AddConstructorArgumentList);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            ObjectCreationExpressionSyntax syntax = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ObjectCreationExpressionSyntax>();

            if (syntax == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Add argument list",
                cancellationToken => AddEmptyArgumentListAsync(context.Document, syntax, cancellationToken),
                DiagnosticIdentifiers.AddConstructorArgumentList + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> AddEmptyArgumentListAsync(
            Document document,
            ObjectCreationExpressionSyntax objectCreationExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ObjectCreationExpressionSyntax newNode = objectCreationExpression
                .WithType(objectCreationExpression.Type.WithoutTrailingTrivia())
                .WithArgumentList(SyntaxFactory
                    .ArgumentList()
                    .WithTrailingTrivia(objectCreationExpression.Type.GetTrailingTrivia()));

            SyntaxNode newRoot = oldRoot.ReplaceNode(objectCreationExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
