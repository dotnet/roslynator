// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RemoveBracesFromStatementRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, StatementSyntax statement)
        {
            if (CanRefactor(context, statement))
            {
#if DEBUG
                context.RegisterRefactoring(
                    $"Remove braces from {SyntaxHelper.GetSyntaxNodeName(statement.Parent)}",
                    cancellationToken => RefactorAsync(context.Document, ((BlockSyntax)statement), cancellationToken));
#else
                context.RegisterRefactoring(
                    $"Remove braces from {SyntaxHelper.GetSyntaxNodeName(statement.Parent.Parent)}",
                    cancellationToken => RefactorAsync(context.Document, statement, cancellationToken));
#endif
            }
        }

        public static bool CanRefactor(RefactoringContext context, StatementSyntax statement)
        {
#if DEBUG
            if (statement.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement;

                if (EmbeddedStatementAnalysis.IsEmbeddableBlock(block))
                {
                    return block.OpenBraceToken.Span.Contains(context.Span)
                        || block.CloseBraceToken.Span.Contains(context.Span);
                }
            }

            return false;
#else
            return EmbeddedStatementAnalysis.IsEmbeddableStatement(statement);
#endif
        }

#if DEBUG
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
#else
        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var block = (BlockSyntax)statement.Parent;

            StatementSyntax newNode = block.Statements[0].TrimLeadingWhitespace()
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(statement.Parent, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
#endif
    }
}
