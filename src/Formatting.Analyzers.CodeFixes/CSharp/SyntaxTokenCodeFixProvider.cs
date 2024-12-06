// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp;

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
                DiagnosticIdentifiers.AddBlankLineBetweenClosingBraceAndNextStatement,
                DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeConditionalOperator,
                DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeArrowToken,
                DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeEqualsToken,
                DiagnosticIdentifiers.PutAttributeListOnItsOwnLine,
                DiagnosticIdentifiers.AddOrRemoveNewLineBeforeWhileInDoStatement,
                DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];

        switch (diagnostic.Id)
        {
            case DiagnosticIdentifiers.AddBlankLineBetweenClosingBraceAndNextStatement:
            {
                await CodeActionFactory.RegisterCodeActionForBlankLineAsync(context).ConfigureAwait(false);
                break;
            }
            case DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeConditionalOperator:
            {
                await CodeActionFactory.RegisterCodeActionForNewLineAroundTokenAsync(context, token => token.IsParentKind(SyntaxKind.ConditionalExpression)).ConfigureAwait(false);
                break;
            }
            case DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeArrowToken:
            {
                await CodeActionFactory.RegisterCodeActionForNewLineAroundTokenAsync(context, token => token.IsKind(SyntaxKind.EqualsGreaterThanToken)).ConfigureAwait(false);
                break;
            }
            case DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeEqualsToken:
            {
                await CodeActionFactory.RegisterCodeActionForNewLineAroundTokenAsync(context, token => token.IsKind(SyntaxKind.EqualsToken)).ConfigureAwait(false);
                break;
            }
            case DiagnosticIdentifiers.PutAttributeListOnItsOwnLine:
            case DiagnosticIdentifiers.AddOrRemoveNewLineBeforeWhileInDoStatement:
            {
                await CodeActionFactory.RegisterCodeActionForNewLineAsync(context).ConfigureAwait(false);
                break;
            }
            case DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine:
            {
                await CodeActionFactory.RegisterCodeActionForNewLineAsync(context, increaseIndentation: true).ConfigureAwait(false);
                break;
            }
        }
    }
}
