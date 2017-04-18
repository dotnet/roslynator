// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
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

            SyntaxToken identifier = root.FindToken(context.Span.Start);

            Debug.Assert(!identifier.IsKind(SyntaxKind.None), identifier.Kind().ToString());

            CodeAction codeAction = CodeAction.Create(
                $"Declare '{identifier.ValueText}' inside namespace",
                cancellationToken => DeclareTypeInsideNamespaceRefactoring.RefactorAsync(context.Document, (MemberDeclarationSyntax)identifier.Parent, cancellationToken),
                DiagnosticIdentifiers.DeclareTypeInsideNamespace + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}