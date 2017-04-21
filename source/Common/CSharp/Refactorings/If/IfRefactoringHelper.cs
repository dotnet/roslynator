// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal static class IfRefactoringHelper
    {
        public static bool IsEquivalent(SyntaxNode node1, SyntaxNode node2)
        {
            return node1 != null
                && node2 != null
                && node1.IsEquivalentTo(node2, topLevel: false);
        }

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

        public static ExpressionSyntax GetBooleanExpression(ExpressionSyntax condition, ExpressionSyntax expression1, ExpressionSyntax expression2)
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
                                return Negator.LogicallyNegate(condition);
                            case SyntaxKind.FalseLiteralExpression:
                                return expression2;
                            default:
                                return LogicalOrExpression(Negator.LogicallyNegate(condition), expression2);
                        }
                    }
                default:
                    {
                        switch (expression2.Kind())
                        {
                            case SyntaxKind.TrueLiteralExpression:
                                return LogicalOrExpression(Negator.LogicallyNegate(condition), expression1);
                            case SyntaxKind.FalseLiteralExpression:
                                return LogicalAndExpression(condition.Parenthesize().WithSimplifierAnnotation(), expression1.Parenthesize().WithSimplifierAnnotation());
                            default:
                                return LogicalOrExpression(ParenthesizedExpression(LogicalAndExpression(condition, expression1)), expression2);
                        }
                    }
            }
        }
    }
}
