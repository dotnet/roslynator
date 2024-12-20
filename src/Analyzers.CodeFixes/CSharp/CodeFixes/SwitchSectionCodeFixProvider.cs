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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SwitchSectionCodeFixProvider))]
[Shared]
public sealed class SwitchSectionCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                DiagnosticIds.RemoveRedundantDefaultSwitchSection,
                DiagnosticIds.DefaultLabelShouldBeLastLabelInSwitchSection,
                DiagnosticIds.AddBracesToSwitchSectionWithMultipleStatements,
                DiagnosticIds.MergeSwitchSectionsWithEquivalentContent);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out SwitchSectionSyntax switchSection))
            return;

        foreach (Diagnostic diagnostic in context.Diagnostics)
        {
            switch (diagnostic.Id)
            {
                case DiagnosticIds.RemoveRedundantDefaultSwitchSection:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Remove redundant switch section",
                        ct => RemoveRedundantDefaultSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
                case DiagnosticIds.DefaultLabelShouldBeLastLabelInSwitchSection:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Move default label to the last position",
                        ct => DefaultLabelShouldBeLastLabelInSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
                case DiagnosticIds.AddBracesToSwitchSectionWithMultipleStatements:
                {
                    CodeAction codeAction = CodeAction.Create(
                        AddBracesToSwitchSectionRefactoring.Title,
                        ct => AddBracesToSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
                case DiagnosticIds.MergeSwitchSectionsWithEquivalentContent:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Merge sections",
                        ct => MergeSwitchSectionsRefactoring.RefactorAsync(context.Document, switchSection, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            }
        }
    }
}
