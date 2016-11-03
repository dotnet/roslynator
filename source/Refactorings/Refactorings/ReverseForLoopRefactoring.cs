// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReverseForLoopRefactoring
    {
        public static bool CanRefactor(ForStatementSyntax forStatement)
        {
            if (forStatement
                .Declaration?
                .Variables.Count == 1)
            {
                ExpressionSyntax value = forStatement
                    .Declaration
                    .Variables[0]
                    .Initializer?
                    .Value;

                return value?.IsNumericLiteralExpression(0) == true
                    && forStatement.Condition?.IsKind(SyntaxKind.LessThanExpression) == true
                    && forStatement.Incrementors.Count == 1
                    && forStatement.Incrementors[0].IsKind(SyntaxKind.PostIncrementExpression);
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

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
                incrementor.WithOperatorToken(Token(SyntaxKind.MinusMinusToken)));

            ForStatementSyntax newForStatement = forStatement
                .WithDeclaration(newDeclaration)
                .WithCondition(newCondition)
                .WithIncrementors(newIncrementors);

            SyntaxNode newRoot = oldRoot.ReplaceNode(forStatement, newForStatement);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
