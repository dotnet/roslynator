// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MergeSimpleAssignmentWithReturnStatementCodeFixProvider))]
    [Shared]
    public class MergeSimpleAssignmentWithReturnStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.MergeSimpleAssignmentWithReturnStatement);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            var expressionStatement = (ExpressionStatementSyntax)root.DescendantNodes(context.Span)
                .FirstOrDefault(f => f.IsKind(SyntaxKind.ExpressionStatement) && f.Span.Start == context.Span.Start);

            if (expressionStatement == null)
                return;

            var block = (BlockSyntax)expressionStatement.Parent;

            int index = block.Statements.IndexOf(expressionStatement);

            var returnStatement = (ReturnStatementSyntax)block.Statements[index + 1];

            CodeAction codeAction = CodeAction.Create(
                "Merge assignment with return statement",
                cancellationToken =>
                {
                    return MergeSimpleAssignmentWithReturnStatementAsync(
                        context.Document,
                        expressionStatement,
                        returnStatement,
                        block,
                        cancellationToken);
                },
                DiagnosticIdentifiers.MergeSimpleAssignmentWithReturnStatement + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> MergeSimpleAssignmentWithReturnStatementAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            ReturnStatementSyntax returnStatement,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

            ReturnStatementSyntax newReturnStatement = returnStatement
                .WithExpression(assignment.Right.WithoutTrivia())
                .WithLeadingTrivia(expressionStatement.GetLeadingTrivia())
                .WithTrailingTrivia(returnStatement.GetTrailingTrivia())
                .WithAdditionalAnnotations(Formatter.Annotation);

            int index = block.Statements.IndexOf(expressionStatement);

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
