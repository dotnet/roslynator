// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal static class EmbeddedStatementHelper
    {
        public static StatementSyntax GetEmbeddedStatement(
            SyntaxNode node,
            bool ifInsideElse = true,
            bool usingInsideUsing = true)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            StatementSyntax statement = GetChildStatement(node, ifInsideElse, usingInsideUsing);

            if (statement?.IsKind(SyntaxKind.Block) == false)
            {
                return statement;
            }
            else
            {
                return null;
            }
        }

        private static StatementSyntax GetChildStatement(
            SyntaxNode node,
            bool ifInsideElse = true,
            bool usingInsideUsing = true)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.IfStatement:
                    return ((IfStatementSyntax)node).Statement;
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.ForEachVariableStatement:
                    return ((CommonForEachStatementSyntax)node).Statement;
                case SyntaxKind.ForStatement:
                    return ((ForStatementSyntax)node).Statement;
                case SyntaxKind.WhileStatement:
                    return ((WhileStatementSyntax)node).Statement;
                case SyntaxKind.DoStatement:
                    return ((DoStatementSyntax)node).Statement;
                case SyntaxKind.LockStatement:
                    return ((LockStatementSyntax)node).Statement;
                case SyntaxKind.FixedStatement:
                    return ((FixedStatementSyntax)node).Statement;
                case SyntaxKind.UsingStatement:
                    {
                        StatementSyntax statement = ((UsingStatementSyntax)node).Statement;

                        if (usingInsideUsing
                            || statement?.IsKind(SyntaxKind.UsingStatement) != true)
                        {
                            return statement;
                        }

                        break;
                    }
                case SyntaxKind.ElseClause:
                    {
                        StatementSyntax statement = ((ElseClauseSyntax)node).Statement;

                        if (ifInsideElse
                            || statement?.IsKind(SyntaxKind.IfStatement) != true)
                        {
                            return statement;
                        }

                        break;
                    }
            }

            return null;
        }

        internal static BlockSyntax AnalyzeBlockToEmbeddedStatement(
            SyntaxNode node,
            bool ifInsideElse = true,
            bool usingInsideUsing = true)
        {
            if (!(GetChildStatement(node, ifInsideElse, usingInsideUsing) is BlockSyntax block))
                return null;

            StatementSyntax statement = block.Statements.SingleOrDefault(shouldThrow: false);

            if (statement == null)
                return null;

            if (statement.IsKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.LabeledStatement))
                return null;

            if (!statement.IsSingleLine())
                return null;

            if (!CheckFormatting(node))
                return null;

            return block;
        }

        internal static StatementSyntax AnalyzeEmbeddedStatementToBlock(
            SyntaxNode node,
            bool ifInsideElse = true,
            bool usingInsideUsing = true)
        {
            StatementSyntax statement = GetEmbeddedStatement(node, ifInsideElse, usingInsideUsing);

            if (statement == null)
                return null;

            if (statement.IsSingleLine() && CheckFormatting(node))
                return null;

            return statement;
        }

        private static bool CheckFormatting(SyntaxNode containingNode)
        {
            switch (containingNode.Kind())
            {
                case SyntaxKind.IfStatement:
                    {
                        return ((IfStatementSyntax)containingNode).Condition?.IsMultiLine() != true;
                    }
                case SyntaxKind.ElseClause:
                    {
                        return true;
                    }
                case SyntaxKind.DoStatement:
                    {
                        return ((DoStatementSyntax)containingNode).Condition?.IsMultiLine() != true;
                    }
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.ForEachVariableStatement:
                    {
                        var forEachStatement = (CommonForEachStatementSyntax)containingNode;

                        return forEachStatement.SyntaxTree.IsSingleLineSpan(forEachStatement.ParenthesesSpan());
                    }
                case SyntaxKind.ForStatement:
                    {
                        var forStatement = (ForStatementSyntax)containingNode;

                        return forStatement.Statement?.IsKind(SyntaxKind.EmptyStatement) == true
                            || forStatement.SyntaxTree.IsSingleLineSpan(forStatement.ParenthesesSpan());
                    }
                case SyntaxKind.UsingStatement:
                    {
                        return ((UsingStatementSyntax)containingNode).DeclarationOrExpression()?.IsMultiLine() != true;
                    }
                case SyntaxKind.WhileStatement:
                    {
                        var whileStatement = (WhileStatementSyntax)containingNode;

                        return whileStatement.Condition?.IsMultiLine() != true
                            || whileStatement.Statement?.IsKind(SyntaxKind.EmptyStatement) == true;
                    }
                case SyntaxKind.LockStatement:
                    {
                        return ((LockStatementSyntax)containingNode).Expression?.IsMultiLine() != true;
                    }
                case SyntaxKind.FixedStatement:
                    {
                        return ((FixedStatementSyntax)containingNode).Declaration?.IsMultiLine() != true;
                    }
                default:
                    {
                        Debug.Fail(containingNode.Kind().ToString());
                        return false;
                    }
            }
        }
    }
}
