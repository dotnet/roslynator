// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SyntaxTriviaCodeFixProvider))]
[Shared]
public sealed class SyntaxTriviaCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                FormattingDiagnosticIds.AddBlankLineAfterTopComment,
                FormattingDiagnosticIds.AddBlankLineBeforeTopDeclaration,
                FormattingDiagnosticIds.AddBlankLineBetweenAccessors,
                FormattingDiagnosticIds.BlankLineBetweenSingleLineAccessors,
                FormattingDiagnosticIds.BlankLineBetweenUsingDirectives,
                FormattingDiagnosticIds.RemoveBlankLineBetweenUsingDirectivesWithSameRootNamespace,
                FormattingDiagnosticIds.RemoveNewLineBetweenIfKeywordAndElseKeyword,
                FormattingDiagnosticIds.RemoveNewLineBeforeBaseList);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];

        switch (diagnostic.Id)
        {
            case FormattingDiagnosticIds.AddBlankLineBeforeTopDeclaration:
            case FormattingDiagnosticIds.AddBlankLineBetweenAccessors:
            case FormattingDiagnosticIds.BlankLineBetweenSingleLineAccessors:
            case FormattingDiagnosticIds.BlankLineBetweenUsingDirectives:
            case FormattingDiagnosticIds.AddBlankLineAfterTopComment:
            case FormattingDiagnosticIds.RemoveBlankLineBetweenUsingDirectivesWithSameRootNamespace:
            {
                await CodeActionFactory.RegisterCodeActionForBlankLineAsync(context).ConfigureAwait(false);
                return;
            }
            case FormattingDiagnosticIds.RemoveNewLineBeforeBaseList:
            case FormattingDiagnosticIds.RemoveNewLineBetweenIfKeywordAndElseKeyword:
            {
                await CodeActionFactory.RegisterCodeActionForNewLineAsync(context).ConfigureAwait(false);
                break;
            }
        }
    }
}
