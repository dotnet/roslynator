// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CompilationUnitCodeFixProvider))]
    [Shared]
    public sealed class CompilationUnitCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.NormalizeWhitespaceAtBeginningOfFile); }
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
                case DiagnosticIdentifiers.NormalizeWhitespaceAtBeginningOfFile:
                    {
                        SyntaxToken token = compilationUnit.EndOfFileToken;

                        if (token.FullSpan.Start > 0)
                            token = compilationUnit.GetFirstToken();

                        SyntaxTriviaList leading = token.LeadingTrivia;

                        string title;
                        if (leading.First().IsWhitespaceTrivia()
                            && (leading.Count == 1
                                || leading[1].IsEndOfLineTrivia()))
                        {
                            title = "Remove whitespace";
                        }
                        else
                        {
                            title = CodeFixTitles.RemoveNewLine;
                        }

                        CodeAction codeAction = CodeAction.Create(
                            title,
                            ct =>
                            {
                                SyntaxToken token = compilationUnit.EndOfFileToken;

                                if (token.FullSpan.Start > 0)
                                    token = compilationUnit.GetFirstToken();

                                SyntaxTriviaList leading = token.LeadingTrivia;

                                int count = leading.TakeWhile(f => f.IsWhitespaceOrEndOfLineTrivia()).Count();

                                SyntaxToken newToken = token.WithLeadingTrivia(leading.RemoveRange(0, count));

                                return document.ReplaceTokenAsync(token, newToken, ct);
                            },
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }
    }
}
