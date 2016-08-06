// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ReplaceBooleanExpressionWithIfStatementRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ReturnStatementSyntax returnStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            IfStatementSyntax newNode = IfStatement(returnStatement.Expression, Block())
                .WithTriviaFrom(returnStatement)
                .WithFormatterAnnotation();

            root = root.ReplaceNode(returnStatement, newNode);

            return document.WithSyntaxRoot(root);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            YieldStatementSyntax yieldStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            IfStatementSyntax newNode = IfStatement(yieldStatement.Expression, Block())
                .WithTriviaFrom(yieldStatement)
                .WithFormatterAnnotation();

            root = root.ReplaceNode(yieldStatement, newNode);

            return document.WithSyntaxRoot(root);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            IfStatementSyntax newNode = IfStatement(expressionStatement.Expression, Block())
                .WithTriviaFrom(expressionStatement)
                .WithFormatterAnnotation();

            root = root.ReplaceNode(expressionStatement, newNode);

            return document.WithSyntaxRoot(root);
        }
    }
}
