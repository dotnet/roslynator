// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class AddParenthesesRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                    return false;
            }

            switch (expression.Parent?.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.InvocationExpression:
                    return false;
            }

            try
            {
                Refactor(expression, context.Root, context.CancellationToken);
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Debug.Assert(false, nameof(AddParenthesesRefactoring));
                return false;
            }

            return true;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            root = Refactor(expression, root, cancellationToken);

            return document.WithSyntaxRoot(root);
        }

        private static SyntaxNode Refactor(
            ExpressionSyntax expression,
            SyntaxNode root,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ParenthesizedExpressionSyntax newNode = SyntaxFactory.ParenthesizedExpression(expression.WithoutTrivia())
                .WithTriviaFrom(expression);

            return root.ReplaceNode(expression, newNode);
        }
    }
}
