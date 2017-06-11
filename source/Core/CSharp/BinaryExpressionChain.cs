// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal struct BinaryExpressionChain
    {
        private BinaryExpressionChain(BinaryExpressionSyntax originalExpression, IEnumerable<ExpressionSyntax> expressions)
        {
            OriginalExpression = originalExpression;
            Expressions = ImmutableArray.CreateRange(expressions);
        }

        public ImmutableArray<ExpressionSyntax> Expressions { get; }

        public BinaryExpressionSyntax OriginalExpression { get; }

        public static BinaryExpressionChain Create(BinaryExpressionSyntax binaryExpression)
        {
            List<ExpressionSyntax> expressions = GetExpressions(binaryExpression, binaryExpression.Kind());

            expressions.Reverse();

            return new BinaryExpressionChain(binaryExpression, expressions);
        }

        public static bool TryCreate(
            SyntaxNode node,
            SyntaxKind binaryExpressionKind,
            out BinaryExpressionChain result)
        {
            var binaryExpression = node as BinaryExpressionSyntax;

            if (binaryExpression?.Kind() == binaryExpressionKind)
            {
                List<ExpressionSyntax> expressions = GetExpressions(binaryExpression, binaryExpressionKind);

                if (expressions != null)
                {
                    expressions.Reverse();
                    result = new BinaryExpressionChain(binaryExpression, expressions);
                    return true;
                }
            }

            result = default(BinaryExpressionChain);
            return false;
        }

        public static bool TryCreate(
            BinaryExpressionSyntax binaryExpression,
            out BinaryExpressionChain result)
        {
            List<ExpressionSyntax> expressions = GetExpressions(binaryExpression, binaryExpression.Kind());

            if (expressions != null)
            {
                expressions.Reverse();
                result = new BinaryExpressionChain(binaryExpression, expressions);
                return true;
            }

            result = default(BinaryExpressionChain);
            return false;
        }

        private static List<ExpressionSyntax> GetExpressions(
            BinaryExpressionSyntax binaryExpression,
            SyntaxKind kind)
        {
            List<ExpressionSyntax> expressions = null;
            bool success = true;

            while (success)
            {
                success = false;

                if (binaryExpression.Kind() == kind)
                {
                    ExpressionSyntax right = binaryExpression.Right;

                    if (right?.IsMissing == false)
                    {
                        (expressions ?? (expressions = new List<ExpressionSyntax>())).Add(right);

                        ExpressionSyntax left = binaryExpression.Left;

                        if (left?.IsMissing == false)
                        {
                            if (left.Kind() == kind)
                            {
                                binaryExpression = (BinaryExpressionSyntax)left;
                                success = true;
                                continue;
                            }
                            else
                            {
                                expressions.Add(left);
                                return expressions;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public override string ToString()
        {
            return OriginalExpression?.ToString();
        }
    }
}
