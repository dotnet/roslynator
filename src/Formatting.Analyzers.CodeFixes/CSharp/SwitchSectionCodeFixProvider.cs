// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SwitchSectionCodeFixProvider))]
[Shared]
public sealed class SwitchSectionCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                DiagnosticIdentifiers.AddBlankLineBetweenSwitchSections,
                DiagnosticIdentifiers.BlankLineBetweenSwitchSections);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out SwitchSectionSyntax switchSection))
            return;

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        switch (diagnostic.Id)
        {
            case DiagnosticIdentifiers.AddBlankLineBetweenSwitchSections:
                {
                    CodeAction codeAction = CodeAction.Create(
                        CodeFixTitles.AddNewLine,
                        ct => CodeFixHelpers.AppendEndOfLineAsync(document, switchSection, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            case DiagnosticIdentifiers.BlankLineBetweenSwitchSections:
                {
                    TextChange textChange = TriviaBetweenAnalysis.GetTextChangeForBlankLine(root, context.Span.Start);

                    CodeAction codeAction = CodeAction.Create(
                        (textChange.NewText.Length == 0) ? CodeFixTitles.RemoveBlankLine : CodeFixTitles.AddBlankLine,
                        ct => document.WithTextChangeAsync(textChange, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
        }
    }
}
