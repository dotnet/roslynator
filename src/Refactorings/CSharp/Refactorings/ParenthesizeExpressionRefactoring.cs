// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ParenthesizeExpressionRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, ExpressionSyntax expression)
        {
            if (AreParenthesesRedundantOrInvalid(expression)
                && !expression.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                try
                {
                    Refactor(context.Root, expression);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    Debug.Fail($"{nameof(ParenthesizeExpressionRefactoring)}\r\n{expression.Kind()}");
                }
            }

            return false;
        }

        private static bool AreParenthesesRedundantOrInvalid(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.IdentifierName:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                case SyntaxKind.ThrowExpression:
                case SyntaxKind.PredefinedType:
                    return false;
                case SyntaxKind.QualifiedName:
                    return !node.IsParentKind(SyntaxKind.NamespaceDeclaration, SyntaxKind.UsingDirective);
            }

            SyntaxNode parent = node.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                case SyntaxKind.QualifiedName:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.TypeArgumentList:
                case SyntaxKind.TypeParameterList:
                case SyntaxKind.VariableDeclaration:
                case SyntaxKind.AwaitExpression:
                case SyntaxKind.Interpolation:
                case SyntaxKind.ArrayInitializerExpression:
                case SyntaxKind.CollectionInitializerExpression:
                case SyntaxKind.ArrowExpressionClause:
                case SyntaxKind.EqualsValueClause:
                    return false;
                case SyntaxKind.ConditionalAccessExpression:
                    {
                        var conditionalAccess = (ConditionalAccessExpressionSyntax)parent;

                        return node != conditionalAccess.WhenNotNull;
                    }
                case SyntaxKind.ForEachStatement:
                    {
                        var forEachStatement = (ForEachStatementSyntax)parent;

                        return node != forEachStatement.Expression
                            && node != forEachStatement.Type;
                    }
                case SyntaxKind.ForEachVariableStatement:
                    {
                        var forEachStatement = (ForEachVariableStatementSyntax)parent;

                        return node != forEachStatement.Expression
                            && node != forEachStatement.Variable;
                    }
                case SyntaxKind.WhileStatement:
                    return node != ((WhileStatementSyntax)parent).Condition;
                case SyntaxKind.DoStatement:
                    return node != ((DoStatementSyntax)parent).Condition;
                case SyntaxKind.LockStatement:
                    return node != ((LockStatementSyntax)parent).Expression;
                case SyntaxKind.IfStatement:
                    return node != ((IfStatementSyntax)parent).Condition;
                case SyntaxKind.SwitchStatement:
                    return node != ((SwitchStatementSyntax)parent).Expression;
                case SyntaxKind.UsingStatement:
                    {
                        var usingStatement = (UsingStatementSyntax)parent;

                        return node != usingStatement.Expression
                            && node != usingStatement.Declaration;
                    }
                case SyntaxKind.ConditionalExpression:
                    {
                        var conditionalExpression = (ConditionalExpressionSyntax)parent;

                        return node != conditionalExpression.WhenTrue
                            && node != conditionalExpression.WhenFalse;
                    }
            }

            return !(parent is AssignmentExpressionSyntax);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = Refactor(root, expression);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxNode Refactor(SyntaxNode root, ExpressionSyntax expression)
        {
            return root.ReplaceNode(
                expression,
                expression.Parenthesize(simplifiable: false));
        }
    }
}
