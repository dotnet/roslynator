// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    public struct BinaryExpressionInfo
    {
        private static BinaryExpressionInfo Default { get; } = new BinaryExpressionInfo();

        private BinaryExpressionInfo(
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax left,
            ExpressionSyntax right)
        {
            BinaryExpression = binaryExpression;
            Left = left;
            Right = right;
        }

        public BinaryExpressionSyntax BinaryExpression { get; }

        public ExpressionSyntax Left { get; }

        public ExpressionSyntax Right { get; }

        public SyntaxKind Kind
        {
            get { return BinaryExpression?.Kind() ?? SyntaxKind.None; }
        }

        public bool Success
        {
            get { return BinaryExpression != null; }
        }

        internal static BinaryExpressionInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateCore(
                Walk(node, walkDownParentheses) as BinaryExpressionSyntax,
                walkDownParentheses,
                allowMissing);
        }

        internal static BinaryExpressionInfo Create(
            BinaryExpressionSyntax binaryExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateCore(binaryExpression, walkDownParentheses, allowMissing);
        }

        internal static BinaryExpressionInfo CreateCore(
            BinaryExpressionSyntax binaryExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (binaryExpression == null)
                return Default;

            ExpressionSyntax left = Walk(binaryExpression.Left, walkDownParentheses);

            if (!Check(left, allowMissing))
                return Default;

            ExpressionSyntax right = Walk(binaryExpression.Right, walkDownParentheses);

            if (!Check(right, allowMissing))
                return Default;

            return new BinaryExpressionInfo(binaryExpression, left, right);
        }

        public override string ToString()
        {
            return BinaryExpression?.ToString() ?? base.ToString();
        }
    }
}