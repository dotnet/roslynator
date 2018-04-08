// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeIfStatementWithNestedIfStatementRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IfStatementSyntax nestedIf = MergeIfStatementWithNestedIfStatementAnalyzer.GetNestedIfStatement(ifStatement);

            ExpressionSyntax left = ifStatement.Condition.Parenthesize();
            ExpressionSyntax right = nestedIf.Condition;

            if (!right.IsKind(SyntaxKind.LogicalAndExpression))
                right = right.Parenthesize();

            BinaryExpressionSyntax newCondition = CSharpFactory.LogicalAndExpression(left, right);

            IfStatementSyntax newNode = GetNewIfStatement(ifStatement, nestedIf)
                .WithCondition(newCondition)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(ifStatement, newNode, cancellationToken);
        }

        private static IfStatementSyntax GetNewIfStatement(IfStatementSyntax ifStatement, IfStatementSyntax ifStatement2)
        {
            if (ifStatement.Statement.IsKind(SyntaxKind.Block))
            {
                if (ifStatement2.Statement.IsKind(SyntaxKind.Block))
                {
                    return ifStatement.ReplaceNode(ifStatement2, ((BlockSyntax)ifStatement2.Statement).Statements);
                }
                else
                {
                    return ifStatement.ReplaceNode(ifStatement2, ifStatement2.Statement);
                }
            }
            else
            {
                return ifStatement.ReplaceNode(ifStatement.Statement, ifStatement2.Statement);
            }
        }
    }
}
