// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp
{
    internal class SelectedExpressions
    {
        private SelectedExpressions(BinaryExpressionSyntax binaryExpression, List<ExpressionSyntax> expressions, TextSpan span)
        {
            BinaryExpression = binaryExpression;
            Expressions = ImmutableArray.CreateRange(expressions);
            Span = span;
        }

        public BinaryExpressionSyntax BinaryExpression { get; }
        public ImmutableArray<ExpressionSyntax> Expressions { get; }
        public TextSpan Span { get; }

        public static SelectedExpressions TryCreate(BinaryExpressionSyntax binaryExpression, TextSpan span)
        {
            List<ExpressionSyntax> expressions = GetExpressions(binaryExpression, span);

            if (expressions != null)
                return new SelectedExpressions(binaryExpression, expressions, span);

            return null;
        }

        private static List<ExpressionSyntax> GetExpressions(BinaryExpressionSyntax binaryExpression, TextSpan span)
        {
            SyntaxKind kind = binaryExpression.Kind();

            List<ExpressionSyntax> expressions = null;
            bool success = true;

            while (success)
            {
                success = false;

                ExpressionSyntax right = binaryExpression.Right;

                if (right?.IsMissing == false
                    && span.Contains(right.Span))
                {
                    if (expressions == null)
                        expressions = new List<ExpressionSyntax>();

                    expressions.Add(right);

                    if (span.Start >= right.FullSpan.Start
                        && span.Start <= right.SpanStart)
                    {
                        expressions.Reverse();
                        return expressions;
                    }
                    else
                    {
                        ExpressionSyntax left = binaryExpression.Left;

                        if (left?.IsMissing == false)
                        {
                            var leftBinaryExpression = left as BinaryExpressionSyntax;

                            if (leftBinaryExpression?.Kind() == kind)
                            {
                                binaryExpression = leftBinaryExpression;
                                success = true;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
