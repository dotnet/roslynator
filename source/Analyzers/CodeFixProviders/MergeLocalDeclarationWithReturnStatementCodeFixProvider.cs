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

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MergeLocalDeclarationWithReturnStatementCodeFixProvider))]
    [Shared]
    public class MergeLocalDeclarationWithReturnStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.MergeLocalDeclarationWithReturnStatement);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var localDeclaration = (LocalDeclarationStatementSyntax)root.DescendantNodes(context.Span)
                .FirstOrDefault(f => f.IsKind(SyntaxKind.LocalDeclarationStatement) && f.Span.Start == context.Span.Start);

            if (localDeclaration == null)
                return;

            var block = (BlockSyntax)localDeclaration.Parent;

            int index = block.Statements.IndexOf(localDeclaration);

            var returnStatement = (ReturnStatementSyntax)block.Statements[index + 1];

            CodeAction codeAction = CodeAction.Create(
                "Merge local declaration with return statement",
                cancellationToken =>
                {
                    return MergeLocalDeclarationWithReturnStatementAsync(
                        context.Document,
                        localDeclaration,
                        returnStatement,
                        block,
                        cancellationToken);
                },
                DiagnosticIdentifiers.MergeLocalDeclarationWithReturnStatement + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> MergeLocalDeclarationWithReturnStatementAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            ReturnStatementSyntax returnStatement,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ReturnStatementSyntax newReturnStatement = returnStatement
                .WithExpression(localDeclaration.Declaration.Variables[0].Initializer.Value.WithoutTrivia())
                .WithLeadingTrivia(localDeclaration.GetLeadingTrivia())
                .WithTrailingTrivia(returnStatement.GetTrailingTrivia())
                .WithFormatterAnnotation();

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
