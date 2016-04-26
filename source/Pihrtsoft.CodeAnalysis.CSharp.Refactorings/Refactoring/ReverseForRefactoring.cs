// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ReverseForRefactoring
    {
        public static bool CanRefactor(ForStatementSyntax forStatement)
        {
            //check declaration

            ExpressionSyntax value = forStatement
                .Declaration?
                .Variables.SingleOrDefault()?
                .Initializer?
                .Value;

            if (value?.IsKind(SyntaxKind.NumericLiteralExpression) != true)
                return false;

            if (((LiteralExpressionSyntax)value).Token.ValueText != "0")
                return false;

            // check condition

            if (forStatement.Condition?.IsKind(SyntaxKind.LessThanExpression) != true)
                return false;

            // check incrementor

            if (forStatement.Incrementors.SingleOrDefault()?.IsKind(SyntaxKind.PostIncrementExpression) != true)
                return false;

            return true;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

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
