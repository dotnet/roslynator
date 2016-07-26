// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters
{
    internal class InterpolatedStringSyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly ImmutableArray<ExpressionSyntax> _expandedArguments;

        private InterpolatedStringSyntaxRewriter(ImmutableArray<ExpressionSyntax> expandedArguments)
        {
            _expandedArguments = expandedArguments;
        }

        public static InterpolatedStringExpressionSyntax VisitNode(
            InterpolatedStringExpressionSyntax interpolatedString,
            ImmutableArray<ExpressionSyntax> expandedArguments)
        {
            return (InterpolatedStringExpressionSyntax)new InterpolatedStringSyntaxRewriter(expandedArguments).Visit(interpolatedString);
        }

        public override SyntaxNode VisitInterpolation(InterpolationSyntax node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var literalExpression = node.Expression as LiteralExpressionSyntax;

            if (literalExpression != null && literalExpression.IsKind(SyntaxKind.NumericLiteralExpression))
            {
                var index = (int)literalExpression.Token.Value;

                if (index >= 0 && index < _expandedArguments.Length)
                    return node.WithExpression(_expandedArguments[index]);
            }

            return base.VisitInterpolation(node);
        }
    }
}
