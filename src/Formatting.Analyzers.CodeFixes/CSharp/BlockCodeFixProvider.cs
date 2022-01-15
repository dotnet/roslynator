// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BlockCodeFixProvider))]
    [Shared]
    public sealed class BlockCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.FormatBlockBraces,
                    DiagnosticIdentifiers.AddNewLineAfterOpeningBraceOfEmptyBlock);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out BlockSyntax block))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.FormatBlockBraces:
                case DiagnosticIdentifiers.AddNewLineAfterOpeningBraceOfEmptyBlock:
                    {
                        bool isSingleLine = block.IsSingleLine(includeExteriorTrivia: false);
                        string title = (isSingleLine)
                            ? "Format braces on multiple lines"
                            : "Format braces on a single line";

                        CodeAction codeAction = CodeAction.Create(
                            title,
                            ct => (isSingleLine)
                                ? FormatBlockBracesOnMultipleLinesAsync(document, block, ct)
                                : FormatBlockBracesOnSingleLineAsync(document, block, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> FormatBlockBracesOnSingleLineAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            BlockSyntax newBlock = block
                .RemoveWhitespace(block.Span)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }

        private static Task<Document> FormatBlockBracesOnMultipleLinesAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            BlockSyntax newBlock = block
                .WithCloseBraceToken(block.CloseBraceToken.AppendEndOfLineToLeadingTrivia())
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }
    }
}
