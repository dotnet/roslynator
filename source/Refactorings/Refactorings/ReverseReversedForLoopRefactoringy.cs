// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReverseReversedForLoopRefactoring
    {
        public static bool CanRefactor(ForStatementSyntax forStatement)
        {
            if (forStatement.Declaration?.Variables.Count == 1)
            {
                ExpressionSyntax value = forStatement
                    .Declaration
                    .Variables[0]
                    .Initializer?
                    .Value;

                return value?.IsKind(SyntaxKind.SubtractExpression) == true
                    && ((BinaryExpressionSyntax)value).Right?.IsNumericLiteralExpression(1) == true
                    && forStatement.Condition?.IsKind(SyntaxKind.GreaterThanOrEqualExpression) == true
                    && ((BinaryExpressionSyntax)forStatement.Condition).Right?.IsNumericLiteralExpression(0) == true
                    && forStatement.Incrementors.Count == 1
                    && forStatement.Incrementors[0].IsKind(SyntaxKind.PostDecrementExpression);
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            VariableDeclarationSyntax declaration = forStatement.Declaration;

            var incrementor = (PostfixUnaryExpressionSyntax)forStatement.Incrementors[0];

            var initializerValue = (BinaryExpressionSyntax)declaration.Variables[0].Initializer.Value;

            VariableDeclarationSyntax newDeclaration = declaration.ReplaceNode(
                initializerValue,
                ZeroLiteralExpression());

            BinaryExpressionSyntax newCondition = ((BinaryExpressionSyntax)forStatement.Condition)
                .WithOperatorToken(Token(SyntaxKind.LessThanToken))
                .WithRight(initializerValue.Left);

            SeparatedSyntaxList<ExpressionSyntax> newIncrementors = forStatement.Incrementors.Replace(
                incrementor,
                incrementor.WithOperatorToken(Token(SyntaxKind.PlusPlusToken)));

            ForStatementSyntax newForStatement = forStatement
                .WithDeclaration(newDeclaration)
                .WithCondition(newCondition)
                .WithIncrementors(newIncrementors);

            root = root.ReplaceNode(forStatement, newForStatement);

            return document.WithSyntaxRoot(root);
        }
    }
}
