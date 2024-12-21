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
                DiagnosticIds.AddBlankLineAfterTopComment,
                DiagnosticIds.AddBlankLineBeforeTopDeclaration,
                DiagnosticIds.AddBlankLineBetweenAccessors,
                DiagnosticIds.BlankLineBetweenSingleLineAccessors,
                DiagnosticIds.BlankLineBetweenUsingDirectives,
                DiagnosticIds.RemoveBlankLineBetweenUsingDirectivesWithSameRootNamespace,
                DiagnosticIds.RemoveNewLineBetweenIfKeywordAndElseKeyword,
                DiagnosticIds.RemoveNewLineBeforeBaseList);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];

        switch (diagnostic.Id)
        {
            case DiagnosticIds.AddBlankLineBeforeTopDeclaration:
            case DiagnosticIds.AddBlankLineBetweenAccessors:
            case DiagnosticIds.BlankLineBetweenSingleLineAccessors:
            case DiagnosticIds.BlankLineBetweenUsingDirectives:
            case DiagnosticIds.AddBlankLineAfterTopComment:
            case DiagnosticIds.RemoveBlankLineBetweenUsingDirectivesWithSameRootNamespace:
            {
                await CodeActionFactory.RegisterCodeActionForBlankLineAsync(context).ConfigureAwait(false);
                return;
            }
            case DiagnosticIds.RemoveNewLineBeforeBaseList:
            case DiagnosticIds.RemoveNewLineBetweenIfKeywordAndElseKeyword:
            {
                await CodeActionFactory.RegisterCodeActionForNewLineAsync(context).ConfigureAwait(false);
                break;
            }
        }
    }
}
