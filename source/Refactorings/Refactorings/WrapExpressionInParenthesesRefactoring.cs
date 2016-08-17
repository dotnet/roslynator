// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
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

            if (IsConditionOrExpression(expression)
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

        private static bool IsConditionOrExpression(ExpressionSyntax expression)
        {
            SyntaxNode parent = expression.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ForEachStatement:
                    return expression == ((ForEachStatementSyntax)parent).Expression;
                case SyntaxKind.WhileStatement:
                    return expression == ((WhileStatementSyntax)parent).Condition;
                case SyntaxKind.DoStatement:
                    return expression == ((DoStatementSyntax)parent).Condition;
                case SyntaxKind.UsingStatement:
                    return expression == ((UsingStatementSyntax)parent).Expression;
                case SyntaxKind.LockStatement:
                    return expression == ((LockStatementSyntax)parent).Expression;
                case SyntaxKind.IfStatement:
                    return expression == ((IfStatementSyntax)parent).Condition;
                case SyntaxKind.SwitchStatement:
                    return expression == ((SwitchStatementSyntax)parent).Expression;
            }

            return false;
        }

        private static bool IsVariableDeclarationValue(ExpressionSyntax expression)
        {
            SyntaxNode parent = expression.Parent;

            if (parent?.IsKind(SyntaxKind.EqualsValueClause) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.VariableDeclarator) == true)
                {
                    parent = parent.Parent;

                    if (parent?.IsKind(SyntaxKind.VariableDeclaration) == true)
                        return true;
                }
            }

            return false;
        }
    }
}
