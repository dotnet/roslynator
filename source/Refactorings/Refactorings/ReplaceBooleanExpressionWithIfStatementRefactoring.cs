// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class ReplaceBooleanExpressionWithIfStatementRefactoring
    {
        public static void RegisterRefactoring(RefactoringContext context, ReturnStatementSyntax returnStatement)
        {
            RegisterRefactoring(context, returnStatement.Expression);
        }

        public static void RegisterRefactoring(RefactoringContext context, YieldStatementSyntax yieldStatement)
        {
            RegisterRefactoring(context, yieldStatement.Expression);
        }

        private static void RegisterRefactoring(RefactoringContext context, ExpressionSyntax expression)
        {
            context.RegisterRefactoring(
                $"Replace statement with 'if ({expression.ToString()})'",
                cancellationToken => RefactorAsync(context.Document, expression, cancellationToken));
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            IfStatementSyntax newNode = IfStatement(expression, Block())
                .WithTriviaFrom(expression.Parent)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(expression.Parent, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
