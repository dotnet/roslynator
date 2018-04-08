// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReverseReversedForLoopRefactoring
    {
        public static bool CanRefactor(ForStatementSyntax forStatement)
        {
            ExpressionSyntax value = forStatement
                .Declaration?
                .Variables
                .SingleOrDefault(shouldThrow: false)?
                .Initializer?
                .Value;

            if (value?.Kind() != SyntaxKind.SubtractExpression)
                return false;

            if (((BinaryExpressionSyntax)value).Right?.IsNumericLiteralExpression("1") != true)
                return false;

            ExpressionSyntax condition = forStatement.Condition;

            if (condition?.Kind() != SyntaxKind.GreaterThanOrEqualExpression)
                return false;

            if (((BinaryExpressionSyntax)condition).Right?.IsNumericLiteralExpression("0") != true)
                return false;

            return forStatement.Incrementors.SingleOrDefault(shouldThrow: false)?.Kind() == SyntaxKind.PostDecrementExpression;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VariableDeclarationSyntax declaration = forStatement.Declaration;

            var incrementor = (PostfixUnaryExpressionSyntax)forStatement.Incrementors[0];

            var initializerValue = (BinaryExpressionSyntax)declaration.Variables[0].Initializer.Value;

            VariableDeclarationSyntax newDeclaration = declaration.ReplaceNode(
                initializerValue,
                NumericLiteralExpression(0));

            BinaryExpressionSyntax newCondition = ((BinaryExpressionSyntax)forStatement.Condition)
                .WithOperatorToken(LessThanToken())
                .WithRight(initializerValue.Left);

            SeparatedSyntaxList<ExpressionSyntax> newIncrementors = forStatement.Incrementors.Replace(
                incrementor,
                incrementor.WithOperatorToken(PlusPlusToken()));

            ForStatementSyntax newForStatement = forStatement
                .WithDeclaration(newDeclaration)
                .WithCondition(newCondition)
                .WithIncrementors(newIncrementors);

            return document.ReplaceNodeAsync(forStatement, newForStatement, cancellationToken);
        }
    }
}
