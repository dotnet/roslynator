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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CompositeEnumValueContainsUndefinedFlagCodeFixProvider))]
    [Shared]
    public class CompositeEnumValueContainsUndefinedFlagCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.CompositeEnumValueContainsUndefinedFlag); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            EnumDeclarationSyntax enumDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<EnumDeclarationSyntax>();

            Debug.Assert(enumDeclaration != null, $"{enumDeclaration} is null");

            if (enumDeclaration == null)
                return;

            string value = context.Diagnostics[0].Properties["Value"];

            CodeAction codeAction = CodeAction.Create(
                $"Declare enum member with value {value}",
                cancellationToken => CompositeEnumValueContainsUndefinedFlagRefactoring.RefactorAsync(context.Document, enumDeclaration, value, cancellationToken),
                DiagnosticIdentifiers.CompositeEnumValueContainsUndefinedFlag + BaseCodeFixProvider.EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
