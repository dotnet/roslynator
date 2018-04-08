// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReverseForLoopRefactoring
    {
        public static bool CanRefactor(ForStatementSyntax forStatement)
        {
            ExpressionSyntax value = forStatement
                .Declaration?
                .Variables
                .SingleOrDefault(shouldThrow: false)?
                .Initializer?
                .Value;

            if (value?.IsNumericLiteralExpression("0") != true)
                return false;

            if (forStatement.Condition?.Kind() != SyntaxKind.LessThanExpression)
                return false;

            return forStatement.Incrementors.SingleOrDefault(shouldThrow: false)?.Kind() == SyntaxKind.PostIncrementExpression;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VariableDeclarationSyntax declaration = forStatement.Declaration;

            var incrementor = (PostfixUnaryExpressionSyntax)forStatement.Incrementors[0];

            VariableDeclarationSyntax newDeclaration = declaration.ReplaceNode(
                declaration.Variables[0].Initializer.Value,
                SubtractExpression(
                    ((BinaryExpressionSyntax)forStatement.Condition).Right,
                    NumericLiteralExpression(1)));

            BinaryExpressionSyntax newCondition = GreaterThanOrEqualExpression(
                ((BinaryExpressionSyntax)forStatement.Condition).Left,
                NumericLiteralExpression(0));

            SeparatedSyntaxList<ExpressionSyntax> newIncrementors = forStatement.Incrementors.Replace(
                incrementor,
                incrementor.WithOperatorToken(MinusMinusToken()));

            ForStatementSyntax newForStatement = forStatement
                .WithDeclaration(newDeclaration)
                .WithCondition(newCondition)
                .WithIncrementors(newIncrementors);

            return document.ReplaceNodeAsync(forStatement, newForStatement, cancellationToken);
        }
    }
}
