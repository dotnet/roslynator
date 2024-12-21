// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FileScopedNamespaceDeclarationCodeFixProvider))]
[Shared]
public sealed class FileScopedNamespaceDeclarationCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIds.BlankLineAfterFileScopedNamespaceDeclaration); }
    }

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        return CodeActionFactory.RegisterCodeActionForBlankLineAsync(context);
    }
}
