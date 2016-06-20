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
            if (expression.IsKind(SyntaxKind.ParenthesizedExpression))
                return false;

            if (expression.IsKind(SyntaxKind.Argument))
                return false;

            if (expression.Parent != null)
            {
                if (expression.Parent.IsKind(SyntaxKind.ParenthesizedExpression))
                    return false;

                if (expression.IsKind(SyntaxKind.IdentifierName) && expression.Parent.IsKind(SyntaxKind.Argument))
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
#if DEBUG
            catch (Exception ex)
            {
                Debug.Assert(false, ex.GetType().ToString());
            }
#endif
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
