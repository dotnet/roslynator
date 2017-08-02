// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CompositeEnumValueContainsUndefinedFlagCodeFixProvider))]
    [Shared]
    public class CompositeEnumValueContainsUndefinedFlagCodeFixProvider : AbstractCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.CompositeEnumValueContainsUndefinedFlag); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out EnumDeclarationSyntax enumDeclaration))
                return;

            string value = context.Diagnostics[0].Properties["Value"];

            CodeAction codeAction = CodeAction.Create(
                $"Declare enum member with value {value}",
                cancellationToken => CompositeEnumValueContainsUndefinedFlagRefactoring.RefactorAsync(context.Document, enumDeclaration, value, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.CompositeEnumValueContainsUndefinedFlag));

            context.RegisterCodeFix(codeAction, context.Diagnostics[0]);
        }
    }
}
