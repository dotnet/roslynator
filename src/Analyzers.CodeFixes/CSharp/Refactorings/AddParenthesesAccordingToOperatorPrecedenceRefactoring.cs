// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddParenthesesAccordingToOperatorPrecedenceRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var newNode = (ExpressionSyntax)SyntaxRewriter.Instance.Visit(expression);

            newNode = newNode.Parenthesize(simplifiable: false);

            return document.ReplaceNodeAsync(expression, newNode, cancellationToken);
        }

        private class SyntaxRewriter : CSharpSyntaxRewriter
        {
            private SyntaxRewriter()
            {
            }

            public static SyntaxRewriter Instance { get; } = new SyntaxRewriter();

            public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
            {
                ExpressionSyntax left = VisitExpression(node.Left);
                ExpressionSyntax right = VisitExpression(node.Right);

                return node.Update(left, node.OperatorToken, right);
            }

            private ExpressionSyntax VisitExpression(ExpressionSyntax expression)
            {
                bool isFixable = AddParenthesesAccordingToOperatorPrecedenceAnalyzer.IsFixable(expression);

                expression = (ExpressionSyntax)base.Visit(expression);

                if (isFixable)
                    expression = expression.Parenthesize(simplifiable: false);

                return expression;
            }
        }
    }
}