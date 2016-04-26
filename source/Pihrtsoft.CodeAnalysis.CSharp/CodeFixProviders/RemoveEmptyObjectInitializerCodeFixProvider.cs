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
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveEmptyObjectInitializerCodeFixProvider))]
    [Shared]
    public class RemoveEmptyObjectInitializerCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.RemoveEmptyObjectInitializer);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            InitializerExpressionSyntax node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InitializerExpressionSyntax>();

            if (node == null)
                return;

            var objectCreationExpression = (ObjectCreationExpressionSyntax)node.Parent;

            CodeAction codeAction = CodeAction.Create(
                "Remove object initializer",
                cancellationToken => RemoveEmptyObjectInitializerAsync(context.Document, objectCreationExpression, cancellationToken),
                DiagnosticIdentifiers.RemoveEmptyObjectInitializer + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> RemoveEmptyObjectInitializerAsync(
            Document document,
            ObjectCreationExpressionSyntax objectCreationExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ObjectCreationExpressionSyntax newObjectCreationExpression = objectCreationExpression
                .WithInitializer(null);

            if (newObjectCreationExpression.ArgumentList == null)
            {
                newObjectCreationExpression = newObjectCreationExpression
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList()
                            .WithTrailingTrivia(objectCreationExpression.Type.GetTrailingTrivia()));
            }

            newObjectCreationExpression = newObjectCreationExpression
                .WithTriviaFrom(objectCreationExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(objectCreationExpression, newObjectCreationExpression);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
