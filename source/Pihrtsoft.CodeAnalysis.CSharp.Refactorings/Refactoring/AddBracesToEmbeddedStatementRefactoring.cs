// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class AddBracesToEmbeddedStatementRefactoring
    {
        public static bool CanRefactor(StatementSyntax statement)
        {
            return EmbeddedStatementAnalysis.IsEmbeddedStatement(statement);
        }

        public static void Refactor(CodeRefactoringContext context, StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            if (CanRefactor(statement))
            {
                context.RegisterRefactoring(
                    $"Add braces to {SyntaxHelper.GetSyntaxNodeName(statement.Parent)}",
                    cancellationToken => RefactorAsync(context.Document, statement, cancellationToken));
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            BlockSyntax block = SyntaxFactory.Block(statement)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(statement, block);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
