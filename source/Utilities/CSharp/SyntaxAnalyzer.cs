// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public static class SyntaxAnalyzer
    {
        public static bool AreParenthesesRedundantOrInvalid(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return AreParenthesesRedundantOrInvalidPrivate(node, node.Kind());
        }

        public static bool AreParenthesesRedundantOrInvalid(SyntaxNode node, SyntaxKind replacementKind)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return AreParenthesesRedundantOrInvalidPrivate(node, replacementKind);
        }

        private static bool AreParenthesesRedundantOrInvalidPrivate(SyntaxNode node, SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.IdentifierName:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                    return true;
            }

            SyntaxNode parent = node.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                case SyntaxKind.QualifiedName:
                //case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.ConditionalAccessExpression:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.TypeArgumentList:
                case SyntaxKind.VariableDeclaration:
                case SyntaxKind.AwaitExpression:
                case SyntaxKind.Interpolation:
                case SyntaxKind.CollectionInitializerExpression:
                case SyntaxKind.ArrowExpressionClause:
                    return true;
                case SyntaxKind.ForEachStatement:
                    {
                        var forEachStatement = (ForEachStatementSyntax)parent;

                        return node == forEachStatement.Expression
                            || node == forEachStatement.Type;
                    }
                case SyntaxKind.WhileStatement:
                    return node == ((WhileStatementSyntax)parent).Condition;
                case SyntaxKind.DoStatement:
                    return node == ((DoStatementSyntax)parent).Condition;
                case SyntaxKind.UsingStatement:
                    return node == ((UsingStatementSyntax)parent).Expression;
                case SyntaxKind.LockStatement:
                    return node == ((LockStatementSyntax)parent).Expression;
                case SyntaxKind.IfStatement:
                    return node == ((IfStatementSyntax)parent).Condition;
                case SyntaxKind.SwitchStatement:
                    return node == ((SwitchStatementSyntax)parent).Expression;
                case SyntaxKind.ConditionalExpression:
                    {
                        var conditionalExpression = (ConditionalExpressionSyntax)parent;

                        return node == conditionalExpression.WhenTrue
                            || node == conditionalExpression.WhenFalse;
                    }
            }

            if (parent is AssignmentExpressionSyntax)
                return true;

            if (parent?.IsKind(SyntaxKind.EqualsValueClause) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.VariableDeclarator) == true)
                {
                    parent = parent.Parent;

                    if (parent?.IsKind(SyntaxKind.VariableDeclaration) == true)
                        return true;
                }
                else if (parent?.IsKind(SyntaxKind.Parameter) == true)
                {
                    return true;
                }
            }

            return false;
        }

        public static BracesAnalysisResult AnalyzeSwitchSection(SwitchSectionSyntax section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            SyntaxList<StatementSyntax> statements = section.Statements;

            if (statements.Count > 1)
            {
                return BracesAnalysisResult.AddBraces;
            }
            else if (statements.Count == 1)
            {
                if (statements[0].IsKind(SyntaxKind.Block))
                {
                    return BracesAnalysisResult.RemoveBraces;
                }
                else
                {
                    return BracesAnalysisResult.AddBraces;
                }
            }

            return BracesAnalysisResult.None;
        }
    }
}
