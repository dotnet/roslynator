// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactorings;

namespace Pihrtsoft.CodeAnalysis.CSharp.Internal.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AwaitExpressionCodeFixProvider))]
    [Shared]
    public class AwaitExpressionCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddConfigureAwait); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context
                .Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            AwaitExpressionSyntax awaitExpressionSyntax = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<AwaitExpressionSyntax>();

            CodeAction codeAction = CodeAction.Create(
                "Add 'ConfigureAwait(false)'",
                cancellationToken => AddConfigureAwaitRefactoring.RefactorAsync(context.Document, awaitExpressionSyntax, cancellationToken),
                DiagnosticIdentifiers.AddConfigureAwait);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
