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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumDeclarationCodeFixProvider))]
    [Shared]
    public class EnumDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.FormatEachEnumMemberOnSeparateLine); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            EnumDeclarationSyntax enumDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<EnumDeclarationSyntax>();

            if (enumDeclaration == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Format each enum member on a separate line",
                cancellationToken => FormatEachEnumMemberOnSeparateLineRefactoring.RefactorAsync(context.Document, enumDeclaration, cancellationToken),
                DiagnosticIdentifiers.FormatEachEnumMemberOnSeparateLine + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
