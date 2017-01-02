// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReverseForLoopRefactoring
    {
        public static bool CanRefactor(ForStatementSyntax forStatement)
        {
            ExpressionSyntax value = forStatement
                .Declaration?
                .SingleVariableOrDefault()?
                .Initializer?
                .Value;

            if (value?.IsNumericLiteralExpression(0) == true
                && forStatement.Condition?.IsKind(SyntaxKind.LessThanExpression) == true)
            {
                SeparatedSyntaxList<ExpressionSyntax> incrementors = forStatement.Incrementors;

                if (incrementors.Count == 1
                    && incrementors[0].IsKind(SyntaxKind.PostIncrementExpression))
                {
                    return true;
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VariableDeclarationSyntax declaration = forStatement.Declaration;

            var incrementor = (PostfixUnaryExpressionSyntax)forStatement.Incrementors[0];

            VariableDeclarationSyntax newDeclaration = declaration.ReplaceNode(
                declaration.Variables[0].Initializer.Value,
                BinaryExpression(
                    SyntaxKind.SubtractExpression,
                    ((BinaryExpressionSyntax)forStatement.Condition).Right,
                    LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(1))));

            BinaryExpressionSyntax newCondition = BinaryExpression(
                SyntaxKind.GreaterThanOrEqualExpression,
                ((BinaryExpressionSyntax)forStatement.Condition).Left,
                LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0)));

            SeparatedSyntaxList<ExpressionSyntax> newIncrementors = forStatement.Incrementors.Replace(
                incrementor,
                incrementor.WithOperatorToken(MinusMinusToken()));

            ForStatementSyntax newForStatement = forStatement
                .WithDeclaration(newDeclaration)
                .WithCondition(newCondition)
                .WithIncrementors(newIncrementors);

            return await document.ReplaceNodeAsync(forStatement, newForStatement, cancellationToken).ConfigureAwait(false);
        }
    }
}
