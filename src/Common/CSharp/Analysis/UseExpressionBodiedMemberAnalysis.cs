// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Analysis
{
    internal static class UseExpressionBodiedMemberAnalysis
    {
        public static bool IsFixable(MethodDeclarationSyntax declaration)
        {
            return declaration.ExpressionBody == null
                && GetExpression(declaration.Body) != null;
        }

        public static bool IsFixable(ConstructorDeclarationSyntax declaration)
        {
            return declaration.ExpressionBody == null
                && GetExpression(declaration.Body) != null;
        }

        public static bool IsFixable(DestructorDeclarationSyntax declaration)
        {
            return declaration.ExpressionBody == null
                && GetExpression(declaration.Body) != null;
        }

        public static bool IsFixable(LocalFunctionStatementSyntax localFunctionStatement)
        {
            return localFunctionStatement.ExpressionBody == null
                && GetExpression(localFunctionStatement.Body) != null;
        }

        public static bool IsFixable(OperatorDeclarationSyntax declaration)
        {
            return declaration.ExpressionBody == null
                && GetReturnExpression(declaration.Body) != null;
        }

        public static bool IsFixable(ConversionOperatorDeclarationSyntax declaration)
        {
            return declaration.ExpressionBody == null
                && GetReturnExpression(declaration.Body) != null;
        }

        public static bool IsFixable(AccessorDeclarationSyntax accessor)
        {
            return accessor.ExpressionBody == null
                && !accessor.AttributeLists.Any()
                && GetExpression(accessor.Body) != null;
        }

        public static ExpressionSyntax GetReturnExpression(AccessorListSyntax accessorList)
        {
            AccessorDeclarationSyntax accessor = accessorList?.Accessors.SingleOrDefault(shouldThrow: false);

            if (accessor?.Kind() == SyntaxKind.GetAccessorDeclaration
                && !accessor.AttributeLists.Any())
            {
                return GetReturnExpression(accessor.Body);
            }

            return default(ExpressionSyntax);
        }

        public static ExpressionSyntax GetReturnExpression(BlockSyntax block)
        {
            StatementSyntax statement = block?.Statements.SingleOrDefault(shouldThrow: false);

            switch (statement?.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    return ((ReturnStatementSyntax)statement).Expression;
                case SyntaxKind.ThrowStatement:
                    return ((ThrowStatementSyntax)statement).Expression;
            }

            return default(ExpressionSyntax);
        }

        public static ExpressionSyntax GetExpression(BlockSyntax block)
        {
            StatementSyntax statement = block?.Statements.SingleOrDefault(shouldThrow: false);

            switch (statement?.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    return ((ReturnStatementSyntax)statement).Expression;
                case SyntaxKind.ExpressionStatement:
                    return ((ExpressionStatementSyntax)statement).Expression;
                case SyntaxKind.ThrowStatement:
                    return ((ThrowStatementSyntax)statement).Expression;
            }

            return default(ExpressionSyntax);
        }

        public static ExpressionSyntax GetExpressionOrThrowExpression(BlockSyntax block)
        {
            StatementSyntax statement = block?.Statements.SingleOrDefault(shouldThrow: false);

            switch (statement?.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    return ((ReturnStatementSyntax)statement).Expression;
                case SyntaxKind.ExpressionStatement:
                    return ((ExpressionStatementSyntax)statement).Expression;
                case SyntaxKind.ThrowStatement:
                    return ThrowExpression(((ThrowStatementSyntax)statement).Expression);
            }

            return default(ExpressionSyntax);
        }
    }
}
