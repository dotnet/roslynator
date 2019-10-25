// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SyntaxTriviaCodeFixProvider))]
    [Shared]
    internal class SyntaxTriviaCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AddEmptyLineBetweenAccessors,
                    DiagnosticIdentifiers.AddEmptyLineBetweenSingleLineAccessors,
                    DiagnosticIdentifiers.AddEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace,
                    DiagnosticIdentifiers.RemoveEmptyLineBetweenSingleLineAccessors,
                    DiagnosticIdentifiers.RemoveEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace,
                    DiagnosticIdentifiers.RemoveEmptyLineBetweenUsingDirectivesWithSameRootNamespace,
                    DiagnosticIdentifiers.RemoveNewLineBetweenIfKeywordAndElseKeyword,
                    DiagnosticIdentifiers.RemoveNewLineBeforeBaseList,
                    DiagnosticIdentifiers.RemoveNewLineBetweenClosingBraceAndWhileKeyword);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            if (!TryFindTrivia(root, context.Span.Start, out SyntaxTrivia trivia, findInsideTrivia: false))
                return;

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AddEmptyLineBetweenAccessors:
                case DiagnosticIdentifiers.AddEmptyLineBetweenSingleLineAccessors:
                case DiagnosticIdentifiers.AddEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.AddEmptyLine,
                            ct => CodeFixHelpers.AppendEndOfLineAsync(document, trivia.Token, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.RemoveEmptyLineBetweenSingleLineAccessors:
                case DiagnosticIdentifiers.RemoveEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace:
                case DiagnosticIdentifiers.RemoveEmptyLineBetweenUsingDirectivesWithSameRootNamespace:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.RemoveEmptyLine,
                            ct => CodeFixHelpers.RemoveEmptyLinesBeforeAsync(document, trivia.Token, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.RemoveNewLineBeforeBaseList:
                case DiagnosticIdentifiers.RemoveNewLineBetweenClosingBraceAndWhileKeyword:
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
