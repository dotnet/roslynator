// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    internal static class OperatorPrecedence
    {
        public static int Compare(SyntaxNode node1, SyntaxNode node2)
        {
            if (node1 == null)
                throw new ArgumentNullException(nameof(node1));

            if (node2 == null)
                throw new ArgumentNullException(nameof(node2));

            return Compare(node1.Kind(), node2.Kind());
        }

        public static int Compare(SyntaxKind kind1, SyntaxKind kind2)
        {
            return GetPrecedence(kind1) - GetPrecedence(kind2);
        }

        public static bool PrecedenceEquals(SyntaxNode node1, SyntaxNode node2)
        {
            return Compare(node1, node2) == 0;
        }

        public static bool PrecedenceEquals(SyntaxKind kind1, SyntaxKind kind2)
        {
            return Compare(kind1, kind2) == 0;
        }

        public static int GetPrecedence(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return GetPrecedence(node.Kind());
        }

        // http://github.com/dotnet/docs/blob/master/docs/csharp/language-reference/operators/index.md
        public static int GetPrecedence(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.ConditionalAccessExpression:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.ElementAccessExpression:
                case SyntaxKind.PostIncrementExpression:
                case SyntaxKind.PostDecrementExpression:
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.AnonymousObjectCreationExpression:
                case SyntaxKind.ArrayCreationExpression:
                case SyntaxKind.ImplicitArrayCreationExpression:
                case SyntaxKind.StackAllocArrayCreationExpression:
                case SyntaxKind.TypeOfExpression:
                case SyntaxKind.CheckedExpression:
                case SyntaxKind.UncheckedExpression:
                case SyntaxKind.DefaultExpression:
                case SyntaxKind.AnonymousMethodExpression:
                case SyntaxKind.SizeOfExpression:
                case SyntaxKind.PointerMemberAccessExpression:
                    return 1;
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PreDecrementExpression:
                case SyntaxKind.CastExpression:
                case SyntaxKind.AwaitExpression:
                case SyntaxKind.AddressOfExpression:
                case SyntaxKind.PointerIndirectionExpression:
                    return 2;
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                    return 3;
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                    return 4;
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                    return 5;
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.IsExpression:
                case SyntaxKind.AsExpression:
                    return 6;
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    return 7;
                case SyntaxKind.BitwiseAndExpression:
                    return 8;
                case SyntaxKind.ExclusiveOrExpression:
                    return 9;
                case SyntaxKind.BitwiseOrExpression:
                    return 10;
                case SyntaxKind.LogicalAndExpression:
                    return 11;
                case SyntaxKind.LogicalOrExpression:
                    return 12;
                case SyntaxKind.CoalesceExpression:
                    return 13;
                case SyntaxKind.ConditionalExpression:
                    return 14;
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                case SyntaxKind.MultiplyAssignmentExpression:
                case SyntaxKind.DivideAssignmentExpression:
                case SyntaxKind.ModuloAssignmentExpression:
                case SyntaxKind.AndAssignmentExpression:
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                case SyntaxKind.OrAssignmentExpression:
                case SyntaxKind.LeftShiftAssignmentExpression:
                case SyntaxKind.RightShiftAssignmentExpression:
                case SyntaxKind.SimpleLambdaExpression:
                case SyntaxKind.ParenthesizedLambdaExpression:
                    return 15;
                default:
                    return 0;
            }
        }
    }
}
