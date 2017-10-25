// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    public struct ConditionalExpressionInfo
    {
        private static ConditionalExpressionInfo Default { get; } = new ConditionalExpressionInfo();

        private ConditionalExpressionInfo(
            ExpressionSyntax condition,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse)
        {
            Condition = condition;
            WhenTrue = whenTrue;
            WhenFalse = whenFalse;
        }

        public ConditionalExpressionSyntax ConditionalExpression
        {
            get { return Condition?.FirstAncestor<ConditionalExpressionSyntax>(); }
        }

        public ExpressionSyntax Condition { get; }

        public ExpressionSyntax WhenTrue { get; }

        public ExpressionSyntax WhenFalse { get; }

        public bool Success
        {
            get { return Condition != null; }
        }

        internal static ConditionalExpressionInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateCore(
                Walk(node, walkDownParentheses) as ConditionalExpressionSyntax,
                walkDownParentheses,
                allowMissing);
        }

        internal static ConditionalExpressionInfo Create(
            ConditionalExpressionSyntax conditionalExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateCore(conditionalExpression, walkDownParentheses, allowMissing);
        }

        internal static ConditionalExpressionInfo CreateCore(
            ConditionalExpressionSyntax conditionalExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (conditionalExpression == null)
                return Default;

            ExpressionSyntax condition = WalkAndCheck(conditionalExpression.Condition, allowMissing, walkDownParentheses);

            if (condition == null)
                return Default;

            ExpressionSyntax whenTrue = WalkAndCheck(conditionalExpression.WhenTrue, allowMissing, walkDownParentheses);

            if (whenTrue == null)
                return Default;

            ExpressionSyntax whenFalse = WalkAndCheck(conditionalExpression.WhenFalse, allowMissing, walkDownParentheses);

            if (whenFalse == null)
                return Default;

            return new ConditionalExpressionInfo(condition, whenTrue, whenFalse);
        }

        public override string ToString()
        {
            return ConditionalExpression?.ToString() ?? base.ToString();
        }
    }
}