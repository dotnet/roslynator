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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EqualsValueClauseCodeFixProvider))]
    [Shared]
    public class EqualsValueClauseCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveRedundantFieldInitialization); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            EqualsValueClauseSyntax equalsValueClause = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<EqualsValueClauseSyntax>();

            Debug.Assert(equalsValueClause != null, $"{nameof(equalsValueClause)} is null");

            if (equalsValueClause == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove redundant field initialization",
                cancellationToken => RemoveRedundantFieldInitializationRefactoring.RefactorAsync(context.Document, equalsValueClause, cancellationToken),
                DiagnosticIdentifiers.RemoveRedundantFieldInitialization + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
