// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp
{
    // BinaryExpressionSelection
    internal struct BinaryExpressionSelection
    {
        private BinaryExpressionSelection(BinaryExpressionSyntax binaryExpression, ImmutableArray<ExpressionSyntax> expressions, TextSpan span)
        {
            BinaryExpression = binaryExpression;
            Expressions = expressions;
            Span = span;
        }

        public BinaryExpressionSyntax BinaryExpression { get; }
        public ImmutableArray<ExpressionSyntax> Expressions { get; }
        public TextSpan Span { get; }

        public override string ToString()
        {
            return BinaryExpression
                .ToFullString()
                .Substring(Span.Start - BinaryExpression.FullSpan.Start, Span.Length);
        }

        public static BinaryExpressionSelection Create(BinaryExpressionSyntax binaryExpression, TextSpan span)
        {
            if (binaryExpression == null)
                throw new ArgumentNullException(nameof(binaryExpression));

            List<ExpressionSyntax> expressions = GetExpressions(binaryExpression, span);

            return new BinaryExpressionSelection(
                binaryExpression,
                (expressions != null) ? ImmutableArray.CreateRange(expressions) : ImmutableArray<ExpressionSyntax>.Empty,
                span);
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
                    (expressions ?? (expressions = new List<ExpressionSyntax>())).Add(right);

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
