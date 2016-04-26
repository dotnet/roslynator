// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CombineLocalDeclarationAndReturnStatementCodeFixProvider))]
    [Shared]
    public class CombineLocalDeclarationAndReturnStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.MergeLocalDeclarationWithReturnStatement);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            StatementSyntax statement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<StatementSyntax>();

            if (statement == null)
                return;

            var block = (BlockSyntax)statement.Parent;
            int index = block.Statements.IndexOf(statement);

            LocalDeclarationStatementSyntax localDeclaration = null;
            ReturnStatementSyntax returnStatement = null;

            if (statement.IsKind(SyntaxKind.LocalDeclarationStatement))
            {
                localDeclaration = (LocalDeclarationStatementSyntax)statement;
                returnStatement = (ReturnStatementSyntax)block.Statements[index + 1];
            }

            if (statement.IsKind(SyntaxKind.ReturnStatement))
            {
                localDeclaration = (LocalDeclarationStatementSyntax)block.Statements[index - 1];
                returnStatement = (ReturnStatementSyntax)statement;
            }

            Debug.Assert(localDeclaration != null, "");
            Debug.Assert(returnStatement != null, "");

            if (localDeclaration == null || returnStatement == null)
                return;

            if (!CheckTrivia(root, TextSpan.FromBounds(localDeclaration.Span.Start, returnStatement.Span.End)))
                return;

            CodeAction codeAction = CodeAction.Create(
                "Merge local declaration with return statement",
                cancellationToken => CombineLocalDeclarationAndReturnStatementAsync(context.Document, localDeclaration, returnStatement, block, cancellationToken),
                DiagnosticIdentifiers.MergeLocalDeclarationWithReturnStatement + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static bool CheckTrivia(SyntaxNode root, TextSpan span)
        {
            foreach (SyntaxTrivia trivia in root.DescendantTrivia(span, descendIntoTrivia: true))
            {
                if (!trivia.IsWhitespaceOrEndOfLine())
                    return false;
            }

            return true;
        }

        private static async Task<Document> CombineLocalDeclarationAndReturnStatementAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            ReturnStatementSyntax returnStatement,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ReturnStatementSyntax newReturnStatement = returnStatement
                .WithExpression(localDeclaration.Declaration.Variables[0].Initializer.Value.WithoutTrivia())
                .WithLeadingTrivia(localDeclaration.GetLeadingTrivia())
                .WithTrailingTrivia(returnStatement.GetTrailingTrivia())
                .WithAdditionalAnnotations(Formatter.Annotation);

            int index = block.Statements.IndexOf(localDeclaration);

            SyntaxList<StatementSyntax> newStatements = block.Statements
                .RemoveAt(index)
                .RemoveAt(index)
                .Insert(index, newReturnStatement);

            BlockSyntax newBlock = block.WithStatements(newStatements);

            SyntaxNode newRoot = oldRoot.ReplaceNode(block, newBlock);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
