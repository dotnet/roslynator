// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NormalizeWhitespaceAtEndOfFileCodeFixProvider))]
    [Shared]
    public sealed class NormalizeWhitespaceAtEndOfFileCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out CompilationUnitSyntax compilationUnit))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile:
                    {
                        bool preferNewLineAtEndOfFile = document.GetConfigOptions(compilationUnit.SyntaxTree).PreferNewLineAtEndOfFile() ?? false;

                        CodeAction codeAction = CodeAction.Create(
                            (preferNewLineAtEndOfFile) ? CodeFixTitles.AddNewLine : CodeFixTitles.RemoveNewLine,
                            ct =>
                            {
                                SyntaxToken endOfFile = compilationUnit.EndOfFileToken;
                                SyntaxTriviaList leading = endOfFile.LeadingTrivia;
                                SyntaxToken oldToken;
                                SyntaxToken newToken;

                                if (!preferNewLineAtEndOfFile)
                                {
                                    if (leading.Any())
                                    {
                                        SyntaxTrivia last = leading.Last();

                                        if (last.GetStructure() is DirectiveTriviaSyntax directive)
                                        {
                                            SyntaxTriviaList trailing = directive.GetTrailingTrivia();

                                            DirectiveTriviaSyntax newDirective = directive.WithTrailingTrivia(trailing.RemoveAt(trailing.Count - 1));

                                            return document.ReplaceNodeAsync(directive, newDirective, ct);
                                        }
                                        else
                                        {
                                            oldToken = endOfFile;
                                            int index = leading.Count - 1;

                                            for (int i = leading.Count - 2; i >= 0; i--)
                                            {
                                                if (leading[i].IsWhitespaceOrEndOfLineTrivia())
                                                    index--;
                                            }

                                            newToken = oldToken.WithLeadingTrivia(leading.RemoveRange(index, leading.Count - index));
                                        }
                                    }
                                    else
                                    {
                                        oldToken = endOfFile.GetPreviousToken();
                                        SyntaxTriviaList trailing = oldToken.TrailingTrivia;
                                        newToken = oldToken.WithTrailingTrivia(trailing.RemoveAt(trailing.Count - 1));
                                    }
                                }
                                else if (leading.Any())
                                {
                                    oldToken = endOfFile;

                                    if (leading.Span.Start == 0
                                        && leading.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                                    {
                                        newToken = oldToken.WithoutLeadingTrivia();
                                    }
                                    else
                                    {
                                        newToken = oldToken.AppendEndOfLineToLeadingTrivia();
                                    }
                                }
                                else
                                {
                                    oldToken = endOfFile.GetPreviousToken();
                                    newToken = oldToken.AppendEndOfLineToTrailingTrivia();
                                }

                                return document.ReplaceTokenAsync(oldToken, newToken, ct);
                            },
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }
    }
}
