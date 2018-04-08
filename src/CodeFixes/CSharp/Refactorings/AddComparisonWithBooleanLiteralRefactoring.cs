// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddComparisonWithBooleanLiteralRefactoring
    {
        public static bool IsCondition(ExpressionSyntax expression)
        {
            SyntaxNode parent = expression.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.IfStatement:
                    return ((IfStatementSyntax)parent).Condition == expression;
                case SyntaxKind.DoStatement:
                    return ((DoStatementSyntax)parent).Condition == expression;
                case SyntaxKind.WhileStatement:
                    return ((WhileStatementSyntax)parent).Condition == expression;
                case SyntaxKind.ForStatement:
                    return ((ForStatementSyntax)parent).Condition == expression;
                case SyntaxKind.ConditionalExpression:
                    return ((ConditionalExpressionSyntax)parent).Condition == expression;
                default:
                    return false;
            }
        }

        public static string GetTitle(ExpressionSyntax expression)
        {
            return (expression.IsKind(SyntaxKind.LogicalNotExpression)) ? "Add ' == false'" : "Add ' == true'";
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax newNode = CreateNewExpression(expression)
                .WithTriviaFrom(expression)
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(expression, newNode, cancellationToken);
        }

        private static BinaryExpressionSyntax CreateNewExpression(ExpressionSyntax expression)
        {
            if (expression.Kind() == SyntaxKind.LogicalNotExpression)
            {
                var logicalNot = (PrefixUnaryExpressionSyntax)expression;

                return EqualsExpression(
                    logicalNot.Operand.WithoutTrivia(),
                    FalseLiteralExpression());
            }
            else
            {
                return EqualsExpression(
                    expression.WithoutTrivia(),
                    TrueLiteralExpression());
            }
        }
    }
}
