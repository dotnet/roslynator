// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddBracesToStatementCodeFixProvider))]
    [Shared]
    public class AddBracesToStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.AddBracesToStatement);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            StatementSyntax statement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<StatementSyntax>();

            if (statement == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                $"Add braces to {SyntaxHelper.GetSyntaxNodeName(statement.Parent)}",
                cancellationToken => AddBracesToStatementRefactoring.RefactorAsync(context.Document, statement, cancellationToken),
                DiagnosticIdentifiers.AddBracesToStatement + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}