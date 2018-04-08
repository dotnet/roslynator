// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyLogicalNegationRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            PrefixUnaryExpressionSyntax logicalNot,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax newNode = GetNewNode(logicalNot)
                .WithTriviaFrom(logicalNot)
                .WithSimplifierAnnotation();

            return document.ReplaceNodeAsync(logicalNot, newNode, cancellationToken);
        }

        private static ExpressionSyntax GetNewNode(PrefixUnaryExpressionSyntax logicalNot)
        {
            ExpressionSyntax operand = logicalNot.Operand;
            ExpressionSyntax expression = operand.WalkDownParentheses();

            switch (expression.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    {
                        LiteralExpressionSyntax newNode = BooleanLiteralExpression(expression.Kind() == SyntaxKind.FalseLiteralExpression);

                        newNode = newNode.WithTriviaFrom(expression);

                        return operand.ReplaceNode(expression, newNode);
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        return ((PrefixUnaryExpressionSyntax)expression).Operand;
                    }
                case SyntaxKind.EqualsExpression:
                    {
                        var equalsExpression = (BinaryExpressionSyntax)expression;

                        BinaryExpressionSyntax notEqualsExpression = NotEqualsExpression(
                            equalsExpression.Left,
                            ExclamationEqualsToken().WithTriviaFrom(equalsExpression.OperatorToken),
                            equalsExpression.Right);

                        return operand.ReplaceNode(equalsExpression, notEqualsExpression);
                    }
                case SyntaxKind.InvocationExpression:
                    {
                        var invocationExpression = (InvocationExpressionSyntax)expression;

                        var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

                        ExpressionSyntax lambdaExpression = invocationExpression.ArgumentList.Arguments.First().Expression.WalkDownParentheses();

                        SingleParameterLambdaExpressionInfo lambdaInfo = SyntaxInfo.SingleParameterLambdaExpressionInfo(lambdaExpression);

                        var logicalNot2 = (PrefixUnaryExpressionSyntax)SimplifyLogicalNegationAnalyzer.GetReturnExpression(lambdaInfo.Body).WalkDownParentheses();

                        InvocationExpressionSyntax newNode = invocationExpression.ReplaceNode(logicalNot2, logicalNot2.Operand.WithTriviaFrom(logicalNot2));

                        return RefactoringUtility.ChangeInvokedMethodName(newNode, (memberAccessExpression.Name.Identifier.ValueText == "All") ? "Any" : "All");
                    }
            }

            return null;
        }
    }
}
