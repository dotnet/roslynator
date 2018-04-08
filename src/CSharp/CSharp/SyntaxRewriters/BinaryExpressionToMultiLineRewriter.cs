// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxRewriters
{
    internal class BinaryExpressionToMultiLineRewriter : CSharpSyntaxRewriter
    {
        private readonly SyntaxTriviaList _leadingTrivia;

        private BinaryExpressionSyntax _previous;

        public BinaryExpressionToMultiLineRewriter(SyntaxTriviaList leadingTrivia)
        {
            _leadingTrivia = leadingTrivia;
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            if (_previous == null
                || (_previous.Equals(node.Parent) && node.IsKind(_previous.Kind())))
            {
                node = node
                    .WithLeft(node.Left?.TrimTrivia())
                    .WithOperatorToken(node.OperatorToken.WithLeadingTrivia(_leadingTrivia));

                _previous = node;
            }

            return base.VisitBinaryExpression(node);
        }
    }
}
