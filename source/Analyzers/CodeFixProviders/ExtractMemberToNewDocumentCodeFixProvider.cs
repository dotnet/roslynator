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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExtractMemberToNewDocumentCodeFixProvider))]
    [Shared]
    public class ExtractMemberToNewDocumentCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.DeclareEachTypeInSeparateFile); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            MemberDeclarationSyntax memberDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberDeclarationSyntax>();

            string name = ExtractTypeDeclarationToNewDocumentRefactoring.GetIdentifier(memberDeclaration).ValueText;
            string title = ExtractTypeDeclarationToNewDocumentRefactoring.GetTitle(name);

            CodeAction codeAction = CodeAction.Create(
                title,
                cancellationToken => ExtractTypeDeclarationToNewDocumentRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                DiagnosticIdentifiers.DeclareEachTypeInSeparateFile + BaseCodeFixProvider.EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
