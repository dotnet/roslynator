// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.CodeStyle;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConditionalAccessExpressionCodeFixProvider))]
    [Shared]
    public sealed class ConditionalAccessExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeNullConditionalOperator); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ConditionalAccessExpressionSyntax conditionalAccess))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            if (diagnostic.Properties.TryGetValue(nameof(NewLinePosition), out string newLinePositionRaw)
                && Enum.TryParse(newLinePositionRaw, out NewLinePosition newLinePosition)
                && newLinePosition == NewLinePosition.After)
            {
                CodeAction codeAction = CodeAction.Create(
                    $"Place '{conditionalAccess.OperatorToken.ToString()}' on the previous line",
                    ct => CodeFixHelpers.AddNewLineAfterInsteadOfBeforeAsync(
                        document,
                        conditionalAccess.Expression,
                        conditionalAccess.OperatorToken,
                        conditionalAccess.WhenNotNull,
                        ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else
            {
                CodeAction codeAction = CodeAction.Create(
                    $"Place '{conditionalAccess.OperatorToken.ToString()}' on the next line",
                    ct => CodeFixHelpers.AddNewLineBeforeInsteadOfAfterAsync(
                        document,
                        conditionalAccess.Expression,
                        conditionalAccess.OperatorToken,
                        conditionalAccess.WhenNotNull,
                        ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }
    }
}
