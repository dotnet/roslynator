// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DirectiveTriviaCodeFixProvider))]
    [Shared]
    public sealed class DirectiveTriviaCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AddBlankLineAfterRegionDirective,
                    DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out DirectiveTriviaSyntax directiveTrivia, findInsideTrivia: true))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AddBlankLineAfterRegionDirective:
                case DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.AddBlankLine,
                            ct => AddBlankLineAfterRegionDirectiveAndBeforeEndRegionAsync(document, directiveTrivia, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> AddBlankLineAfterRegionDirectiveAndBeforeEndRegionAsync(
            Document document,
            DirectiveTriviaSyntax directiveTrivia,
            CancellationToken cancellationToken)
        {
            switch (directiveTrivia.Kind())
            {
                case SyntaxKind.RegionDirectiveTrivia:
                    return CodeFixHelpers.AddBlankLineAfterDirectiveAsync(document, directiveTrivia, cancellationToken);
                case SyntaxKind.EndRegionDirectiveTrivia:
                    return CodeFixHelpers.AddBlankLineBeforeDirectiveAsync(document, directiveTrivia, cancellationToken);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
