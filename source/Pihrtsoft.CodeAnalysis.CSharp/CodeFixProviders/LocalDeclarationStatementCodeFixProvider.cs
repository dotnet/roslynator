// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalDeclarationStatementCodeFixProvider))]
    [Shared]
    public class LocalDeclarationStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.SplitDeclarationIntoMultipleDeclarations);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            LocalDeclarationStatementSyntax eventFieldDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<LocalDeclarationStatementSyntax>();

            if (eventFieldDeclaration == null)
                return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    "Split declaration into multiple declarations",
                    cancellationToken =>
                    {
                        return SplitVariablesIntoMultipleDeclarationsAsync(
                            context.Document,
                            eventFieldDeclaration,
                            cancellationToken);
                    },
                    DiagnosticIdentifiers.SplitDeclarationIntoMultipleDeclarations + EquivalenceKeySuffix),
                context.Diagnostics);
        }

        private static async Task<Document> SplitVariablesIntoMultipleDeclarationsAsync(
            Document document,
            LocalDeclarationStatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var block = (BlockSyntax)statement.Parent;

            SyntaxList<StatementSyntax> newStatements = block.Statements.ReplaceRange(
                statement,
                CreateDeclarations(statement));

            BlockSyntax newBlock = block.WithStatements(newStatements);

            SyntaxNode newRoot = oldRoot.ReplaceNode(block, newBlock);

            return document.WithSyntaxRoot(newRoot);
        }

        private static IEnumerable<LocalDeclarationStatementSyntax> CreateDeclarations(LocalDeclarationStatementSyntax statement)
        {
            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = statement.Declaration.Variables;

            LocalDeclarationStatementSyntax statement2 = statement.WithoutTrivia();

            for (int i = 0; i < variables.Count; i++)
            {
                LocalDeclarationStatementSyntax newStatement = LocalDeclarationStatement(
                    statement2.Modifiers,
                    VariableDeclaration(
                        statement2.Declaration.Type,
                        SingletonSeparatedList(variables[i])));

                if (i == 0)
                    newStatement = newStatement.WithLeadingTrivia(statement.GetLeadingTrivia());

                if (i == variables.Count - 1)
                    newStatement = newStatement.WithTrailingTrivia(statement.GetTrailingTrivia());

                yield return newStatement.WithAdditionalAnnotations(Formatter.Annotation);
            }
        }
    }
}
