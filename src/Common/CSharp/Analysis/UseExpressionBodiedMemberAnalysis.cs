// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class UseExpressionBodiedMemberAnalysis
    {
        public static ExpressionSyntax GetReturnExpression(BlockSyntax block)
        {
            StatementSyntax statement = block.Statements.SingleOrDefault(shouldThrow: false);

            if (statement == null)
                return null;

            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    return ((ReturnStatementSyntax)statement).Expression;
                case SyntaxKind.ThrowStatement:
                    return ((ThrowStatementSyntax)statement).Expression;
            }

            return null;
        }

        public static ExpressionSyntax GetExpression(BlockSyntax block)
        {
            StatementSyntax statement = block.Statements.SingleOrDefault(shouldThrow: false);

            if (statement == null)
                return null;

            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    return ((ReturnStatementSyntax)statement).Expression;
                case SyntaxKind.ExpressionStatement:
                    return ((ExpressionStatementSyntax)statement).Expression;
                case SyntaxKind.ThrowStatement:
                    return ((ThrowStatementSyntax)statement).Expression;
            }

            return null;
        }
    }
}
