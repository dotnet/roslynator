// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SyntaxTriviaCodeFixProvider))]
    [Shared]
    public sealed class SyntaxTriviaCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AddBlankLineAfterTopComment,
                    DiagnosticIdentifiers.AddBlankLineBeforeTopDeclaration,
                    DiagnosticIdentifiers.AddBlankLineBetweenAccessors,
                    DiagnosticIdentifiers.BlankLineBetweenSingleLineAccessors,
                    DiagnosticIdentifiers.BlankLineBetweenUsingDirectives,
                    DiagnosticIdentifiers.RemoveBlankLineBetweenUsingDirectivesWithSameRootNamespace,
                    DiagnosticIdentifiers.RemoveNewLineBetweenIfKeywordAndElseKeyword,
                    DiagnosticIdentifiers.RemoveNewLineBeforeBaseList);
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
                case DiagnosticIdentifiers.AddBlankLineAfterTopComment:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.AddBlankLine,
                            ct => document.ReplaceTokenAsync(trivia.Token, trivia.Token.AppendEndOfLineToLeadingTrivia(), ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.AddBlankLineBeforeTopDeclaration:
                case DiagnosticIdentifiers.AddBlankLineBetweenAccessors:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.AddBlankLine,
                            ct => CodeFixHelpers.AppendEndOfLineAsync(document, trivia.Token, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.BlankLineBetweenSingleLineAccessors:
                case DiagnosticIdentifiers.BlankLineBetweenUsingDirectives:
                    {
                        if (DiagnosticProperties.ContainsInvert(diagnostic.Properties))
                        {
                            CodeAction codeAction = CodeAction.Create(
                                CodeFixTitles.RemoveBlankLine,
                                ct => CodeFixHelpers.RemoveBlankLinesBeforeAsync(document, trivia.Token, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                        }
                        else
                        {
                            CodeAction codeAction = CodeAction.Create(
                                CodeFixTitles.AddBlankLine,
                                ct => CodeFixHelpers.AppendEndOfLineAsync(document, trivia.Token, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                        }

                        break;
                    }
                case DiagnosticIdentifiers.RemoveBlankLineBetweenUsingDirectivesWithSameRootNamespace:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.RemoveBlankLine,
                            ct => CodeFixHelpers.RemoveBlankLinesBeforeAsync(document, trivia.Token, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.RemoveNewLineBeforeBaseList:
                case DiagnosticIdentifiers.RemoveNewLineBetweenIfKeywordAndElseKeyword:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.RemoveNewLine,
                            ct => CodeFixHelpers.ReplaceTriviaBetweenAsync(document, trivia.Token, trivia.Token.GetNextToken(), cancellationToken: ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }
    }
}
