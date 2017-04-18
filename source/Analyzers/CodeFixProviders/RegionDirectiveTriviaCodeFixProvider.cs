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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RegionDirectiveTriviaCodeFixProvider))]
    [Shared]
    public class RegionDirectiveTriviaCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveEmptyRegion); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            RegionDirectiveTriviaSyntax region = root
                .FindNode(context.Span, findInsideTrivia: true, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<RegionDirectiveTriviaSyntax>();

            CodeAction codeAction = CodeAction.Create(
                "Remove empty region",
                cancellationToken => RemoveEmptyRegionRefactoring.RefactorAsync(context.Document, region, cancellationToken),
                DiagnosticIdentifiers.RemoveEmptyRegion + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
