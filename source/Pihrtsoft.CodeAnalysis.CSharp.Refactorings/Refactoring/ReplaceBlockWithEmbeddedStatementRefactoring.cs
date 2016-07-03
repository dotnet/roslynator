// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ReplaceBlockWithEmbeddedStatementRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, BlockSyntax block)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceBlockWithEmbeddedStatement)
                && CanRefactor(context, block))
            {
                context.RegisterRefactoring(
                    "Replace block with embedded statement",
                    cancellationToken => RefactorAsync(context.Document, block, cancellationToken));
            }
        }

        public static bool CanRefactor(RefactoringContext context, BlockSyntax block)
        {
            return EmbeddedStatementAnalysis.IsEmbeddableBlock(block);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            StatementSyntax newNode = block.Statements[0].TrimLeadingWhitespace()
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(block, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
