// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal readonly struct BlockExpressionAnalysis
    {
        private BlockExpressionAnalysis(StatementSyntax statement, ExpressionSyntax expression, SyntaxToken semicolonToken, SyntaxToken returnOrThrowKeyword)
        {
            Statement = statement;
            Expression = expression;
            SemicolonToken = semicolonToken;
            ReturnOrThrowKeyword = returnOrThrowKeyword;
        }

        public StatementSyntax Statement { get; }

        public ExpressionSyntax Expression { get; }

        public SyntaxToken SemicolonToken { get; }

        public SyntaxToken ReturnOrThrowKeyword { get; }

        public BlockSyntax Block => (Statement != null) ? (BlockSyntax)Statement.Parent : default;

        public bool Success => Expression != null;

        public static bool SupportsExpressionBody(BlockSyntax block, bool allowExpressionStatement = true)
        {
            return Create(block, allowExpressionStatement: allowExpressionStatement).Success;
        }

        public static BlockExpressionAnalysis Create(AccessorListSyntax accessorList)
        {
            return Create(accessorList.Accessors[0]);
        }

        public static BlockExpressionAnalysis Create(AccessorDeclarationSyntax accessor)
        {
            return Create(accessor.Body);
        }

        public static BlockExpressionAnalysis Create(BlockSyntax block, bool allowExpressionStatement = true)
        {
            StatementSyntax statement = block?.Statements.SingleOrDefault(shouldThrow: false);

            if (statement == null)
                return default;

            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    {
                        var returnStatement = (ReturnStatementSyntax)statement;
                        return new BlockExpressionAnalysis(returnStatement, returnStatement.Expression, returnStatement.SemicolonToken, returnStatement.ReturnKeyword);
                    }
                case SyntaxKind.ExpressionStatement:
                    {
                        if (!allowExpressionStatement)
                            return default;

                        var expressionStatement = (ExpressionStatementSyntax)statement;
                        return new BlockExpressionAnalysis(expressionStatement, expressionStatement.Expression, expressionStatement.SemicolonToken, default);
                    }
                case SyntaxKind.ThrowStatement:
                    {
                        var throwStatement = (ThrowStatementSyntax)statement;
                        return new BlockExpressionAnalysis(throwStatement, throwStatement.Expression, throwStatement.SemicolonToken, throwStatement.ThrowKeyword);
                    }
                default:
                    {
                        return default;
                    }
            }
        }
    }
}
