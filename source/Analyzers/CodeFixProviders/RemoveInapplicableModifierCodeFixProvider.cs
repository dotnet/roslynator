// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using System.Threading;
using Roslynator.CSharp;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveInapplicableModifierCodeFixProvider))]
    [Shared]
    public class RemoveInapplicableModifierCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveInapplicableModifier); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxToken token = root.FindToken(context.Span.Start);

            Debug.Assert(!token.IsKind(SyntaxKind.None), token.Kind().ToString());

            if (token.IsKind(SyntaxKind.None))
                return;

            CodeAction codeAction = CodeAction.Create(
                $"Remove modifier '{token}'",
                cancellationToken => RefactorAsync(context.Document, token, cancellationToken),
                DiagnosticIdentifiers.RemoveInapplicableModifier + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private Task<Document> RefactorAsync(
            Document document,
            SyntaxToken token,
            CancellationToken cancellationToken)
        {
            SyntaxNode node = token.Parent;

            SyntaxNode newNode = node.RemoveModifier(token);

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }
    }
}
