// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    internal static class RemoveHelper
    {
        public static SyntaxRemoveOptions DefaultRemoveOptions
        {
            get { return SyntaxRemoveOptions.KeepExteriorTrivia | SyntaxRemoveOptions.KeepUnbalancedDirectives; }
        }

        public static SyntaxRemoveOptions GetRemoveOptions(SyntaxNode node)
        {
            SyntaxRemoveOptions removeOptions = DefaultRemoveOptions;

            if (node.GetLeadingTrivia().IsEmptyOrWhitespace())
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (node.GetTrailingTrivia().IsEmptyOrWhitespace())
                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return removeOptions;
        }

        public static SyntaxRemoveOptions GetRemoveOptions(CSharpSyntaxNode node)
        {
            SyntaxRemoveOptions removeOptions = DefaultRemoveOptions;

            if (node.GetLeadingTrivia().IsEmptyOrWhitespace())
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (node.GetTrailingTrivia().IsEmptyOrWhitespace())
                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return removeOptions;
        }

        public static TNode RemoveCondition<TNode>(
            TNode node,
            ExpressionSyntax expression,
            bool isTrue) where TNode : SyntaxNode
        {
            expression = expression.WalkUpParentheses();

            SyntaxNode parent = expression.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.IfStatement:
                    {
                        var ifStatement = (IfStatementSyntax)parent;

                        if (ifStatement.Condition == expression)
                        {
                            if (isTrue)
                            {
                                return node.RemoveOrReplaceNode(ifStatement, ifStatement.Statement);
                            }
                            else
                            {
                                return node.RemoveOrReplaceNode(ifStatement, ifStatement.Else?.Statement);
                            }
                        }

                        break;
                    }
                case SyntaxKind.DoStatement:
                    {
                        var doStatement = (DoStatementSyntax)parent;

                        if (doStatement.Condition == expression
                            && !isTrue)
                        {
                            return node.RemoveOrReplaceNode(doStatement, doStatement.Statement);
                        }

                        break;
                    }
                case SyntaxKind.WhileStatement:
                    {
                        var whileStatement = (WhileStatementSyntax)parent;

                        if (whileStatement.Condition == expression
                            && !isTrue)
                        {
                            return node.RemoveNode(whileStatement);
                        }

                        break;
                    }
                case SyntaxKind.ForStatement:
                    {
                        var forStatement = (ForStatementSyntax)parent;

                        if (forStatement.Condition == expression)
                        {
                            if (isTrue)
                            {
                                return node.RemoveNode(expression);
                            }
                            else
                            {
                                return node.RemoveNode(forStatement);
                            }
                        }

                        break;
                    }
                case SyntaxKind.ConditionalExpression:
                    {
                        var conditionalExpression = (ConditionalExpressionSyntax)parent;

                        if (conditionalExpression.Condition == expression)
                        {
                            if (isTrue)
                            {
                                return node.RemoveOrReplaceNode(conditionalExpression, conditionalExpression.WhenTrue);
                            }
                            else
                            {
                                return node.RemoveOrReplaceNode(conditionalExpression, conditionalExpression.WhenFalse);
                            }
                        }

                        break;
                    }
                case SyntaxKind.LogicalAndExpression:
                    {
                        var logicalAnd = (BinaryExpressionSyntax)parent;

                        ExpressionSyntax left = logicalAnd.Left;
                        ExpressionSyntax right = logicalAnd.Right;

                        ExpressionSyntax other = (left == expression) ? right : left;

                        if (isTrue)
                        {
                            return node.RemoveOrReplaceNode(logicalAnd, other);
                        }
                        else
                        {
                            return RemoveCondition(node, logicalAnd, isTrue);
                        }
                    }
                case SyntaxKind.LogicalOrExpression:
                    {
                        var logicalOr = (BinaryExpressionSyntax)parent;

                        ExpressionSyntax left = logicalOr.Left;
                        ExpressionSyntax right = logicalOr.Right;

                        ExpressionSyntax other = (left == expression) ? right : left;

                        if (isTrue)
                        {
                            return RemoveCondition(node, logicalOr, isTrue);
                        }
                        else
                        {
                            return node.RemoveOrReplaceNode(logicalOr, other);
                        }
                    }
            }

            return node.ReplaceNode(expression, BooleanLiteralExpression(isTrue));
        }

        public static TRoot RemoveNode<TRoot>(this TRoot root, SyntaxNode node) where TRoot : SyntaxNode
        {
            if (node == null)
                return root;

            if (node is StatementSyntax statement)
                root.RemoveStatement(statement);

            return root.RemoveNode(node, GetRemoveOptions(node));
        }

        public static TRoot RemoveOrReplaceNode<TRoot>(
            this TRoot root,
            SyntaxNode node,
            SyntaxNode newNode = null) where TRoot : SyntaxNode
        {
            if (newNode == null)
                return root.RemoveNode(node);

            if (newNode.IsKind(SyntaxKind.Block))
                return root.ReplaceNode(node, ((BlockSyntax)newNode).Statements);

            return root.ReplaceNode(node, newNode);
        }
    }
}
