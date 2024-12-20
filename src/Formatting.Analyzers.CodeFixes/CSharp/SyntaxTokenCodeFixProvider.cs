// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SyntaxTokenCodeFixProvider))]
[Shared]
public sealed class SyntaxTokenCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                FormattingDiagnosticIds.AddBlankLineBetweenClosingBraceAndNextStatement,
                FormattingDiagnosticIds.PlaceNewLineAfterOrBeforeConditionalOperator,
                FormattingDiagnosticIds.PlaceNewLineAfterOrBeforeArrowToken,
                FormattingDiagnosticIds.PlaceNewLineAfterOrBeforeEqualsToken,
                FormattingDiagnosticIds.PutAttributeListOnItsOwnLine,
                FormattingDiagnosticIds.AddOrRemoveNewLineBeforeWhileInDoStatement);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];

        switch (diagnostic.Id)
        {
            case FormattingDiagnosticIds.AddBlankLineBetweenClosingBraceAndNextStatement:
            {
                await CodeActionFactory.RegisterCodeActionForBlankLineAsync(context).ConfigureAwait(false);
                break;
            }
            case FormattingDiagnosticIds.PlaceNewLineAfterOrBeforeConditionalOperator:
            {
                await CodeActionFactory.RegisterCodeActionForNewLineAroundTokenAsync(context, token => token.IsParentKind(SyntaxKind.ConditionalExpression)).ConfigureAwait(false);
                break;
            }
            case FormattingDiagnosticIds.PlaceNewLineAfterOrBeforeArrowToken:
            {
                await CodeActionFactory.RegisterCodeActionForNewLineAroundTokenAsync(context, token => token.IsKind(SyntaxKind.EqualsGreaterThanToken)).ConfigureAwait(false);
                break;
            }
            case FormattingDiagnosticIds.PlaceNewLineAfterOrBeforeEqualsToken:
            {
                await CodeActionFactory.RegisterCodeActionForNewLineAroundTokenAsync(context, token => token.IsKind(SyntaxKind.EqualsToken)).ConfigureAwait(false);
                break;
            }
            case FormattingDiagnosticIds.PutAttributeListOnItsOwnLine:
            case FormattingDiagnosticIds.AddOrRemoveNewLineBeforeWhileInDoStatement:
            {
                await CodeActionFactory.RegisterCodeActionForNewLineAsync(context).ConfigureAwait(false);
                break;
            }
        }
    }
}
