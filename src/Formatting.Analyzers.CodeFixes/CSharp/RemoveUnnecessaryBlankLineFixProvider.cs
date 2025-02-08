// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, LanguageNames.VisualBasic, Name = nameof(RemoveUnnecessaryBlankLineFixProvider))]
[Shared]
public sealed class RemoveUnnecessaryBlankLineFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveUnnecessaryBlankLine); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);
        TextSpan span = context.Span;

        if (!root.FindTrivia(span.Start).IsWhitespaceOrEndOfLineTrivia())
        {
            Debug.Fail("");
            return;
        }

        Diagnostic diagnostic = context.Diagnostics[0];

        CodeAction codeAction = CodeAction.Create(
            CodeFixTitles.RemoveBlankLine,
            ct => context.Document.WithTextChangeAsync(span, "", ct),
            GetEquivalenceKey(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }
}
