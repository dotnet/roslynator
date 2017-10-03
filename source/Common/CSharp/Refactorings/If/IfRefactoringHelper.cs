// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal static class IfRefactoringHelper
    {
        public static bool IsNullLiteral(SyntaxNode node)
        {
            return node?.IsKind(SyntaxKind.NullLiteralExpression) == true;
        }

        public static bool IsSimpleAssignment(SyntaxNode node)
        {
            return node?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true;
        }

        public static ConditionalExpressionSyntax CreateConditionalExpression(ExpressionSyntax condition, ExpressionSyntax whenTrue, ExpressionSyntax whenFalse)
        {
            if (!condition.IsKind(SyntaxKind.ParenthesizedExpression))
            {
                condition = ParenthesizedExpression(condition.WithoutTrivia())
                    .WithTriviaFrom(condition);
            }

            return ConditionalExpression(condition, whenTrue, whenFalse);
        }

        public static ExpressionSyntax GetBooleanExpression(
            ExpressionSyntax condition,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (expression1.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                    {
                        switch (expression2.Kind())
                        {
                            case SyntaxKind.TrueLiteralExpression:
                                return expression2;
                            case SyntaxKind.FalseLiteralExpression:
                                return condition;
                            default:
                                return LogicalOrExpression(condition, expression2);
                        }
                    }
                case SyntaxKind.FalseLiteralExpression:
                    {
                        switch (expression2.Kind())
                        {
                            case SyntaxKind.TrueLiteralExpression:
                                return CSharpUtility.LogicallyNegate(condition, semanticModel, cancellationToken);
                            case SyntaxKind.FalseLiteralExpression:
                                return expression2;
                            default:
                                return LogicalAndExpression(CSharpUtility.LogicallyNegate(condition, semanticModel, cancellationToken), expression2);
                        }
                    }
                default:
                    {
                        switch (expression2.Kind())
                        {
                            case SyntaxKind.TrueLiteralExpression:
                                return LogicalOrExpression(CSharpUtility.LogicallyNegate(condition, semanticModel, cancellationToken), expression1);
                            case SyntaxKind.FalseLiteralExpression:
                                return LogicalAndExpression(condition, expression1);
                            default:
                                throw new InvalidOperationException();
                        }
                    }
            }
        }

        private static ExpressionSyntax LogicalAndExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return CSharpFactory.LogicalAndExpression(
                left.Parenthesize(),
                right.Parenthesize());
        }

        private static ExpressionSyntax LogicalOrExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return CSharpFactory.LogicalOrExpression(
                left.Parenthesize(),
                right.Parenthesize());
        }
    }
}
