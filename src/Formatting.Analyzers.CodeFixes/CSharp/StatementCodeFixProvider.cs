// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StatementCodeFixProvider))]
[Shared]
public sealed class StatementCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                FormattingDiagnosticIds.PutStatementOnItsOwnLine,
                FormattingDiagnosticIds.PutEmbeddedStatementOnItsOwnLine,
                FormattingDiagnosticIds.AddNewLineAfterSwitchLabel,
                FormattingDiagnosticIds.AddBlankLineAfterEmbeddedStatement);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];

        switch (diagnostic.Id)
        {
            case FormattingDiagnosticIds.PutStatementOnItsOwnLine:
            {
                await CodeActionFactory.RegisterCodeActionForNewLineAsync(context).ConfigureAwait(false);
                break;
            }
            case FormattingDiagnosticIds.PutEmbeddedStatementOnItsOwnLine:
            case FormattingDiagnosticIds.AddNewLineAfterSwitchLabel:
            {
                await CodeActionFactory.RegisterCodeActionForNewLineAsync(context, increaseIndentation: true).ConfigureAwait(false);
                break;
            }
            case FormattingDiagnosticIds.AddBlankLineAfterEmbeddedStatement:
            {
                await CodeActionFactory.RegisterCodeActionForBlankLineAsync(context).ConfigureAwait(false);
                break;
            }
        }
    }
}
