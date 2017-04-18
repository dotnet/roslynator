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
        public static StatementSyntax GetEmbeddedStatement(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            StatementSyntax statement = GetBlockOrEmbeddedStatement(node);

            if (statement?.IsKind(SyntaxKind.Block) == false)
            {
                return statement;
            }
            else
            {
                return null;
            }
        }

        public static bool IsEmbeddableBlock(BlockSyntax block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            if (CanContainEmbeddedStatement(block.Parent))
            {
                SyntaxList<StatementSyntax> statements = block.Statements;

                return statements.Count == 1
                    && !statements[0].IsKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.LabeledStatement);
            }
            else
            {
                return false;
            }
        }

        public static bool IsEmbeddedStatement(StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            if (!statement.IsKind(SyntaxKind.Block))
            {
                SyntaxNode parent = statement.Parent;

                return CanContainEmbeddedStatement(parent)
                    && (!parent.IsKind(SyntaxKind.ElseClause) || !statement.IsKind(SyntaxKind.IfStatement));
            }
            else
            {
                return false;
            }
        }

        public static bool ContainsEmbeddedStatement(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return GetBlockOrEmbeddedStatement(node)?.IsKind(SyntaxKind.Block) == false;
        }

        public static bool CanContainEmbeddedStatement(SyntaxNode node)
        {
            switch (node?.Kind())
            {
                case SyntaxKind.IfStatement:
                case SyntaxKind.ElseClause:
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.ForEachVariableStatement:
                case SyntaxKind.ForStatement:
                case SyntaxKind.UsingStatement:
                case SyntaxKind.WhileStatement:
                case SyntaxKind.DoStatement:
                case SyntaxKind.LockStatement:
                case SyntaxKind.FixedStatement:
                    return true;
                default:
                    return false;
            }
        }

        public static StatementSyntax GetBlockOrEmbeddedStatement(SyntaxNode node)
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
                        if (statement?.IsKind(SyntaxKind.UsingStatement) != true)
                            return statement;

                        break;
                    }
                case SyntaxKind.ElseClause:
                    {
                        var elseClause = (ElseClauseSyntax)node;

                        if (!elseClause.ContinuesWithIf())
                            return elseClause.Statement;

                        break;
                    }
            }

            return null;
        }

        internal static bool FormattingSupportsEmbeddedStatement(SyntaxNode containingNode)
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
                        Debug.Assert(false, containingNode.Kind().ToString());
                        return false;
                    }
            }
        }
    }
}
