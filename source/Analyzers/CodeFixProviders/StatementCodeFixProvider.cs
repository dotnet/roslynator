// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StatementCodeFixProvider))]
    [Shared]
    public class StatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddEmptyLineAfterLastStatementInDoStatement); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            StatementSyntax statement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<StatementSyntax>();

            if (statement == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Add empty line",
                cancellationToken =>
                {
                    return AddEmptyLineAsync(
                        context.Document,
                        statement,
                        cancellationToken);
                },
                DiagnosticIdentifiers.AddEmptyLineAfterLastStatementInDoStatement + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> AddEmptyLineAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxTriviaList trailingTrivia = statement.GetTrailingTrivia();

            int index = trailingTrivia.IndexOf(SyntaxKind.EndOfLineTrivia);

            SyntaxTriviaList newTrailingTrivia = trailingTrivia.Insert(index, CSharpFactory.NewLineTrivia());

            StatementSyntax newStatement = statement.WithTrailingTrivia(newTrailingTrivia);

            SyntaxNode newRoot = root.ReplaceNode(statement, newStatement);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
