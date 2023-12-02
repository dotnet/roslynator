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
                DiagnosticIdentifiers.AddBlankLineAfterTopComment,
                DiagnosticIdentifiers.AddBlankLineBeforeTopDeclaration,
                DiagnosticIdentifiers.AddBlankLineBetweenAccessors,
                DiagnosticIdentifiers.BlankLineBetweenSingleLineAccessors,
                DiagnosticIdentifiers.BlankLineBetweenUsingDirectives,
                DiagnosticIdentifiers.RemoveBlankLineBetweenUsingDirectivesWithSameRootNamespace,
                DiagnosticIdentifiers.RemoveNewLineBetweenIfKeywordAndElseKeyword,
                DiagnosticIdentifiers.RemoveNewLineBeforeBaseList);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];

        switch (diagnostic.Id)
        {
            case DiagnosticIdentifiers.AddBlankLineBeforeTopDeclaration:
            case DiagnosticIdentifiers.AddBlankLineBetweenAccessors:
            case DiagnosticIdentifiers.BlankLineBetweenSingleLineAccessors:
            case DiagnosticIdentifiers.BlankLineBetweenUsingDirectives:
            case DiagnosticIdentifiers.AddBlankLineAfterTopComment:
            case DiagnosticIdentifiers.RemoveBlankLineBetweenUsingDirectivesWithSameRootNamespace:
                {
                    await CodeActionFactory.CreateAndRegisterCodeActionForBlankLineAsync(context).ConfigureAwait(false);
                    return;
                }
            case DiagnosticIdentifiers.RemoveNewLineBeforeBaseList:
            case DiagnosticIdentifiers.RemoveNewLineBetweenIfKeywordAndElseKeyword:
                {
                    await CodeActionFactory.CreateAndRegisterCodeActionForNewLineAsync(context).ConfigureAwait(false);
                    break;
                }
        }
    }
}
