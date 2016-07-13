// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ReplaceEmbeddedStatementWithBlockRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, StatementSyntax statement)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceEmbeddedStatementWithBlock)
                && CanRefactor(statement))
            {
                context.RegisterRefactoring(
                    "Replace embedded statement with block",
                    cancellationToken => RefactorAsync(context.Document, statement, cancellationToken));
            }
        }

        public static bool CanRefactor(StatementSyntax statement)
        {
            return EmbeddedStatementAnalysis.IsEmbeddedStatement(statement);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            BlockSyntax block = SyntaxFactory.Block(statement)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(statement, block);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
