// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp
{
    internal readonly struct BinaryExpressionSelection
    {
        private BinaryExpressionSelection(BinaryExpressionSyntax binaryExpression, ImmutableArray<ExpressionSyntax> expressions, TextSpan span)
        {
            BinaryExpression = binaryExpression;
            Expressions = expressions;
            Span = span;
        }

        private static BinaryExpressionSelection Default { get; } = new BinaryExpressionSelection();

        public BinaryExpressionSyntax BinaryExpression { get; }

        public ImmutableArray<ExpressionSyntax> Expressions { get; }

        public TextSpan Span { get; }

        public bool Success
        {
            get { return BinaryExpression != null; }
        }

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

            ImmutableArray<ExpressionSyntax> expressions = GetExpressions(binaryExpression, span);

            if (expressions.IsDefault)
                return Default;

            return new BinaryExpressionSelection(binaryExpression, expressions, span);
        }

        private static ImmutableArray<ExpressionSyntax> GetExpressions(BinaryExpressionSyntax binaryExpression, TextSpan span)
        {
            SyntaxKind kind = binaryExpression.Kind();

            ImmutableArray<ExpressionSyntax>.Builder builder = null;
            bool success = true;

            while (success)
            {
                success = false;

                ExpressionSyntax right = binaryExpression.Right;

                if (right?.IsMissing == false
                    && span.Contains(right.Span))
                {
                    (builder ?? (builder = ImmutableArray.CreateBuilder<ExpressionSyntax>())).Add(right);

                    if (span.Start >= right.FullSpan.Start
                        && span.Start <= right.SpanStart)
                    {
                        builder.Reverse();
                        return builder.ToImmutable();
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

            return default(ImmutableArray<ExpressionSyntax>);
        }
    }
}
