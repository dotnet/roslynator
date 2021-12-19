// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.RemoveRedundantStatement
{
    internal sealed class RemoveRedundantReturnStatementAnalysis : RemoveRedundantStatementAnalysis<ReturnStatementSyntax>
    {
        public static RemoveRedundantReturnStatementAnalysis Instance { get; } = new();

        private RemoveRedundantReturnStatementAnalysis()
        {
        }

        public override bool IsFixable(ReturnStatementSyntax statement)
        {
            ExpressionSyntax expression = statement.Expression;

            if (expression == null)
                return base.IsFixable(statement);

            if (expression.IsKind(
                SyntaxKind.NullLiteralExpression,
                SyntaxKind.DefaultLiteralExpression,
                SyntaxKind.TrueLiteralExpression,
                SyntaxKind.FalseLiteralExpression))
            {
                SyntaxNode parent = statement.Parent;

                if (parent.IsKind(SyntaxKind.Block)
                    && parent.Parent is IfStatementSyntax ifStatement
                    && ifStatement.IsSimpleIf())
                {
                    StatementSyntax nextStatement = ifStatement.NextStatement();

                    if (nextStatement.IsKind(SyntaxKind.ReturnStatement)
                        && ((ReturnStatementSyntax)nextStatement).Expression?.RawKind == expression.RawKind)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected override bool IsFixable(StatementSyntax statement, StatementSyntax containingStatement, BlockSyntax block, SyntaxKind parentKind)
        {
            switch (parentKind)
            {
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                    {
                        return true;
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        return ((MethodDeclarationSyntax)block.Parent).ReturnType?.IsVoid() == true;
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        return ((LocalFunctionStatementSyntax)block.Parent).ReturnType?.IsVoid() == true;
                    }
                case SyntaxKind.SimpleLambdaExpression:
                case SyntaxKind.ParenthesizedLambdaExpression:
                case SyntaxKind.AnonymousMethodExpression:
                    {
                        return statement is ReturnStatementSyntax returnStatement
                            && returnStatement.Expression == null;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
    }
}
