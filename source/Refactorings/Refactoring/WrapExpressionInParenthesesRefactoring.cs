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
    internal static class WrapExpressionInParenthesesRefactoring
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
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                    return false;
            }

            if (IsForEachExpression(expression)
                || IsVariableDeclarationValue(expression))
            {
                return false;
            }

            try
            {
                Refactor(expression, context.Root);
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Debug.Assert(false, nameof(WrapExpressionInParenthesesRefactoring));
                return false;
            }

            return true;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            root = Refactor(expression, root);

            return document.WithSyntaxRoot(root);
        }

        private static SyntaxNode Refactor(ExpressionSyntax expression, SyntaxNode root)
        {
            ParenthesizedExpressionSyntax newNode = SyntaxFactory.ParenthesizedExpression(expression.WithoutTrivia())
                .WithTriviaFrom(expression);

            return root.ReplaceNode(expression, newNode);
        }

        private static bool IsForEachExpression(ExpressionSyntax expression)
        {
            return expression.Parent?.IsKind(SyntaxKind.ForEachStatement) == true
                && expression.Equals(((ForEachStatementSyntax)expression.Parent).Expression);
        }

        private static bool IsVariableDeclarationValue(ExpressionSyntax expression)
        {
            return expression.Parent?.IsKind(SyntaxKind.EqualsValueClause) == true
                && expression.Parent.Parent?.IsKind(SyntaxKind.VariableDeclarator) == true
                && expression.Parent.Parent.Parent?.IsKind(SyntaxKind.VariableDeclaration) == true;
        }
    }
}
