// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public static class EmbeddedStatement
    {
        public static StatementSyntax GetEmbeddedStatement(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            StatementSyntax statement = GetBlockOrEmbeddedStatement(node);

            if (statement?.IsKind(SyntaxKind.Block) == false)
                return statement;

            return null;
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

            return false;
        }

        public static bool IsEmbeddedStatement(StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            if (!statement.IsKind(SyntaxKind.Block))
            {
                SyntaxNode parent = statement.Parent;

                if (CanContainEmbeddedStatement(parent)
                    && (!parent.IsKind(SyntaxKind.ElseClause) || !statement.IsKind(SyntaxKind.IfStatement)))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CanContainEmbeddedStatement(SyntaxNode node)
        {
            switch (node?.Kind())
            {
                case SyntaxKind.IfStatement:
                case SyntaxKind.ElseClause:
                case SyntaxKind.ForEachStatement:
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
            switch (node.Kind())
            {
                case SyntaxKind.IfStatement:
                    return ((IfStatementSyntax)node).Statement;
                case SyntaxKind.ForEachStatement:
                    return ((ForEachStatementSyntax)node).Statement;
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

                        if (IfElseChain.IsEndOfChain(elseClause))
                            return elseClause.Statement;

                        break;
                    }
            }

            return null;
        }
    }
}
