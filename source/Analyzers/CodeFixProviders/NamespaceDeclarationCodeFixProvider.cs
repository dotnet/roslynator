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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NamespaceDeclarationCodeFixProvider))]
    [Shared]
    public class NamespaceDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.RemoveEmptyNamespaceDeclaration,
                    DiagnosticIdentifiers.DeclareUsingDirectiveOnTopLevel);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            NamespaceDeclarationSyntax namespaceDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<NamespaceDeclarationSyntax>();

            Debug.Assert(namespaceDeclaration != null, $"{nameof(namespaceDeclaration)} is null");

            if (namespaceDeclaration == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.RemoveEmptyNamespaceDeclaration:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove empty namespace declaration",
                                cancellationToken => RemoveEmptyNamespaceDeclarationRefactoring.RefactorAsync(context.Document, namespaceDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.DeclareUsingDirectiveOnTopLevel:
                        {
                            string title = (namespaceDeclaration.Usings.Count == 1)
                                ? "Move using to top level"
                                : "Move usings to top level";

                            CodeAction codeAction = CodeAction.Create(
                                title,
                                cancellationToken => DeclareUsingDirectiveOnTopLevelRefactoring.RefactorAsync(context.Document, namespaceDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
