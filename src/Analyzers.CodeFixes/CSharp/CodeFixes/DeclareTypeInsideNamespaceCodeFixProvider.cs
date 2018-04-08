// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DeclareTypeInsideNamespaceCodeFixProvider))]
    [Shared]
    public class DeclareTypeInsideNamespaceCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.DeclareTypeInsideNamespace); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindToken(root, context.Span.Start, out SyntaxToken identifier))
                return;

            CodeAction codeAction = CodeAction.Create(
                $"Declare '{identifier.ValueText}' inside namespace",
                cancellationToken => DeclareTypeInsideNamespaceRefactoring.RefactorAsync(context.Document, (MemberDeclarationSyntax)identifier.Parent, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.DeclareTypeInsideNamespace));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}