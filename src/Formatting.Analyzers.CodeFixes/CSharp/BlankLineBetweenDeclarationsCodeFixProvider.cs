// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BlankLineBetweenDeclarationsCodeFixProvider))]
    [Shared]
    public sealed class BlankLineBetweenDeclarationsCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AddBlankLineBetweenDeclarations,
                    DiagnosticIdentifiers.AddBlankLineBetweenSingleLineDeclarations,
                    DiagnosticIdentifiers.AddBlankLineBetweenDeclarationAndDocumentationComment,
                    DiagnosticIdentifiers.AddBlankLineBetweenSingleLineDeclarationsOfDifferentKind,
                    DiagnosticIdentifiers.RemoveBlankLineBetweenSingleLineDeclarationsOfSameKind);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            if (!TryFindTrivia(root, context.Span.Start, out SyntaxTrivia trivia, findInsideTrivia: false))
                return;

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AddBlankLineBetweenDeclarations:
                case DiagnosticIdentifiers.AddBlankLineBetweenSingleLineDeclarations:
                case DiagnosticIdentifiers.AddBlankLineBetweenDeclarationAndDocumentationComment:
                case DiagnosticIdentifiers.AddBlankLineBetweenSingleLineDeclarationsOfDifferentKind:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.AddBlankLine,
                            ct => CodeFixHelpers.AppendEndOfLineAsync(document, trivia.Token, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.RemoveBlankLineBetweenSingleLineDeclarationsOfSameKind:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.RemoveBlankLine,
                            ct => CodeFixHelpers.RemoveBlankLinesBeforeAsync(document, trivia.Token, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }
    }
}
