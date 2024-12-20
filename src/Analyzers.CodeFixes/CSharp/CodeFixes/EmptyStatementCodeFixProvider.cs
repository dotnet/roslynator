// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EmptyStatementCodeFixProvider))]
[Shared]
public sealed class EmptyStatementCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIds.RemoveEmptyStatement); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out EmptyStatementSyntax emptyStatement))
            return;

        CodeAction codeAction = CodeAction.Create(
            "Remove empty statement",
            ct => RemoveEmptyStatementRefactoring.RefactorAsync(context.Document, emptyStatement, ct),
            GetEquivalenceKey(DiagnosticIds.RemoveEmptyStatement));

        context.RegisterCodeFix(codeAction, context.Diagnostics);
    }
}
