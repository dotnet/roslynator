// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumDeclarationCodeFixProvider))]
[Shared]
public sealed class EnumDeclarationCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIds.PutEnumMemberOnItsOwnLine); }
    }

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        return CodeActionFactory.RegisterCodeActionForNewLineAsync(context);
    }
}
