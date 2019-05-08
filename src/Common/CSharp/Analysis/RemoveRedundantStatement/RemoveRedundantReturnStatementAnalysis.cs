// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.RemoveRedundantStatement
{
    internal sealed class RemoveRedundantReturnStatementAnalysis : RemoveRedundantStatementAnalysis<ReturnStatementSyntax>
    {
        public static RemoveRedundantReturnStatementAnalysis Instance { get; } = new RemoveRedundantReturnStatementAnalysis();

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

                if (parent.IsKind(SyntaxKind.Block))
                {
                    parent = parent.Parent;

                    if (parent.IsKind(SyntaxKind.IfStatement))
                    {
                        var ifStatement = (IfStatementSyntax)parent;

                        if (ifStatement.IsSimpleIf())
                        {
                            StatementSyntax nextStatement = ifStatement.NextStatement();

                            if (nextStatement.IsKind(SyntaxKind.ReturnStatement)
                                && ((ReturnStatementSyntax)nextStatement).Expression?.RawKind == expression.RawKind)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        protected override bool IsFixable(StatementSyntax statement, BlockSyntax block, SyntaxKind parentKind)
        {
            if (!parentKind.Is(
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.DestructorDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.SetAccessorDeclaration,
                SyntaxKind.LocalFunctionStatement))
            {
                return false;
            }

            if (parentKind == SyntaxKind.MethodDeclaration)
                return ((MethodDeclarationSyntax)block.Parent).ReturnType?.IsVoid() == true;

            if (parentKind == SyntaxKind.LocalFunctionStatement)
                return ((LocalFunctionStatementSyntax)block.Parent).ReturnType?.IsVoid() == true;

            return true;
        }
    }
}
