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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumMemberDeclarationCodeFixProvider))]
    [Shared]
    public class EnumMemberDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.EnumMemberShouldDeclareExplicitValue,
                    DiagnosticIdentifiers.DeclareEnumValueAsCombinationOfNames);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            EnumMemberDeclarationSyntax enumMemberDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<EnumMemberDeclarationSyntax>();

            if (enumMemberDeclaration == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.EnumMemberShouldDeclareExplicitValue:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Declare explicit value",
                                cancellationToken => EnumMemberShouldDeclareExplicitValueRefactoring.RefactorAsync(context.Document, enumMemberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.DeclareEnumValueAsCombinationOfNames:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Declare value as combination of names",
                                cancellationToken => DeclareEnumValueAsCombinationOfNamesRefactoring.RefactorAsync(context.Document, enumMemberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
