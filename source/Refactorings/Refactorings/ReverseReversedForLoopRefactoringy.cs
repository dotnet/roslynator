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
                .SingleOrDefault(shouldthrow: false)?
                .Initializer?
                .Value;

            if (value?.IsKind(SyntaxKind.SubtractExpression) == true
                && ((BinaryExpressionSyntax)value).Right?.IsNumericLiteralExpression("1") == true)
            {
                ExpressionSyntax condition = forStatement.Condition;

                if (condition?.IsKind(SyntaxKind.GreaterThanOrEqualExpression) == true
                    && ((BinaryExpressionSyntax)condition).Right?.IsNumericLiteralExpression("0") == true)
                {
                    SeparatedSyntaxList<ExpressionSyntax> incrementors = forStatement.Incrementors;

                    if (incrementors.Count == 1
                        && incrementors[0].IsKind(SyntaxKind.PostDecrementExpression))
                    {
                        return true;
                    }
                }
            }

            return false;
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
