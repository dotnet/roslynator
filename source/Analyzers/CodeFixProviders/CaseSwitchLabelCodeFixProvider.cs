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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CaseSwitchLabelCodeFixProvider))]
    [Shared]
    public class CaseSwitchLabelCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveUnnecessaryCaseLabel); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            CaseSwitchLabelSyntax label = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<CaseSwitchLabelSyntax>();

            if (label == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove unnecessary case label",
                cancellationToken =>
                {
                    return RemoveUnnecessaryCaseLabelRefactoring.RefactorAsync(
                        context.Document,
                        label,
                        cancellationToken);
                },
                DiagnosticIdentifiers.RemoveUnnecessaryCaseLabel + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
