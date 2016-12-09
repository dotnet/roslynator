// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveRedundantSealedModifierCodeFixProvider))]
    [Shared]
    public class RemoveRedundantSealedModifierCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveRedundantSealedModifier); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            MemberDeclarationSyntax memberDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberDeclarationSyntax>();

            if (memberDeclaration == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove sealed modifier",
                cancellationToken => RemoveRedundantSealedModifierRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                DiagnosticIdentifiers.RemoveRedundantSealedModifier + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
