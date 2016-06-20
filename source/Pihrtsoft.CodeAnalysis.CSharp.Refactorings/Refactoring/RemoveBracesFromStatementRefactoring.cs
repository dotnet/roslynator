// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RemoveBracesFromStatementRefactoring
    {
        public static bool CanRefactor(StatementSyntax statement)
        {
            return EmbeddedStatementAnalysis.IsEmbeddableStatement(statement);
        }

        public static void Refactor(RefactoringContext context, StatementSyntax statement)
        {
            if (CanRefactor(statement))
            {
                context.RegisterRefactoring(
                    $"Remove braces from {SyntaxHelper.GetSyntaxNodeName(statement.Parent.Parent)}",
                    cancellationToken => RefactorAsync(context.Document, statement, cancellationToken));
            }
        }

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
    }
}
