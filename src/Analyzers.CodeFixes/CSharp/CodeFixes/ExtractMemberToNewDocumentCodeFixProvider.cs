// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExtractMemberToNewDocumentCodeFixProvider))]
    [Shared]
    public sealed class ExtractMemberToNewDocumentCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.DeclareEachTypeInSeparateFile); }
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return null;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out MemberDeclarationSyntax memberDeclaration))
                return;

            string name = CSharpUtility.GetIdentifier(memberDeclaration).ValueText;
            string title = ExtractTypeDeclarationToNewDocumentRefactoring.GetTitle(name);

            CodeAction codeAction = CodeAction.Create(
                title,
                ct => ExtractTypeDeclarationToNewDocumentRefactoring.RefactorAsync(context.Document, memberDeclaration, ct),
                GetEquivalenceKey(DiagnosticIdentifiers.DeclareEachTypeInSeparateFile));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
