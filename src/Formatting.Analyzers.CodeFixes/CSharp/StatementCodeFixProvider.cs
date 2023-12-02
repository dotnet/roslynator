// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                DiagnosticIdentifiers.AddNewLineBeforeStatement,
                DiagnosticIdentifiers.AddNewLineBeforeEmbeddedStatement,
                DiagnosticIdentifiers.AddNewLineAfterSwitchLabel,
                DiagnosticIdentifiers.AddBlankLineAfterEmbeddedStatement);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];

        switch (diagnostic.Id)
        {
            case DiagnosticIdentifiers.AddNewLineBeforeStatement:
                {
                    await CodeActionFactory.CreateAndRegisterCodeActionForNewLineAsync(context).ConfigureAwait(false);
                    break;
                }
            case DiagnosticIdentifiers.AddNewLineBeforeEmbeddedStatement:
            case DiagnosticIdentifiers.AddNewLineAfterSwitchLabel:
                {
                    await CodeActionFactory.CreateAndRegisterCodeActionForNewLineAsync(context, options: CodeActionNewLineOptions.IncreaseIndentation).ConfigureAwait(false);
                    break;
                }
            case DiagnosticIdentifiers.AddBlankLineAfterEmbeddedStatement:
                {
                    await CodeActionFactory.CreateAndRegisterCodeActionForBlankLineAsync(context).ConfigureAwait(false);
                    break;
                }
        }
    }
}
