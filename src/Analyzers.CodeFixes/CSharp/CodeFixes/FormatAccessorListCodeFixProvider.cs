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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FormatAccessorListCodeFixProvider))]
    [Shared]
    public class FormatAccessorListCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.FormatAccessorList); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out AccessorListSyntax accessorList))
                return;

            CodeAction codeAction = CodeAction.Create(
                "Format accessor list",
                cancellationToken => FormatAccessorListRefactoring.RefactorAsync(context.Document, accessorList, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.FormatAccessorList));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
