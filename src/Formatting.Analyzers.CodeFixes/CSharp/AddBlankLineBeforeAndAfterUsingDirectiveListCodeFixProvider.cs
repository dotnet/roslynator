// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddBlankLineBeforeAndAfterUsingDirectiveListCodeFixProvider))]
[Shared]
public sealed class AddBlankLineBeforeAndAfterUsingDirectiveListCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                DiagnosticIdentifiers.AddBlankLineBeforeUsingDirectiveList,
                DiagnosticIdentifiers.AddBlankLineAfterUsingDirectiveList);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        await CodeActionFactory.RegisterCodeActionForBlankLineAsync(context).ConfigureAwait(false);
    }
}
