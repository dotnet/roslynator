// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    internal struct BinaryExpressionChainInfo
    {
        private static BinaryExpressionChainInfo Default { get; } = new BinaryExpressionChainInfo();

        private BinaryExpressionChainInfo(
            BinaryExpressionSyntax binaryExpression,
            SyntaxKind kind,
            IEnumerable<ExpressionSyntax> expressions)
        {
            BinaryExpression = binaryExpression;
            Kind = kind;
            Expressions = ImmutableArray.CreateRange(expressions);
        }

        public BinaryExpressionSyntax BinaryExpression { get; }

        public SyntaxKind Kind { get; }

        public ImmutableArray<ExpressionSyntax> Expressions { get; }

        public bool Success
        {
            get { return BinaryExpression != null; }
        }

        internal static BinaryExpressionChainInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true)
        {
            return Create(Walk(node, walkDownParentheses) as BinaryExpressionSyntax);
        }

        internal static BinaryExpressionChainInfo Create(BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression == null)
                return Default;

            return CreateCore(binaryExpression, binaryExpression.Kind());
        }

        private static BinaryExpressionChainInfo CreateCore(BinaryExpressionSyntax binaryExpression, SyntaxKind kind)
        {
            List<ExpressionSyntax> expressions = GetExpressions(binaryExpression, kind);

            if (expressions == null)
                return Default;

            expressions.Reverse();

            return new BinaryExpressionChainInfo(binaryExpression, kind, expressions);
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
            return BinaryExpression?.ToString();
        }
    }
}
