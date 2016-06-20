// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class LambdaExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, LambdaExpressionSyntax lambda)
        {
            var expression = lambda.Body as ExpressionSyntax;

            if (expression != null
                && expression.Span.Contains(context.Span)
                && context.SupportsSemanticModel)
            {
                context.RegisterRefactoring(
                    "Expand lambda expression's body",
                    cancellationToken =>
                    {
                        return ExpandLambdaExpressionBodyAsync(
                            context.Document,
                            lambda,
                            expression,
                            cancellationToken);
                    });
            }
        }

        private static async Task<Document> ExpandLambdaExpressionBodyAsync(
            Document document,
            LambdaExpressionSyntax lambda,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            ISymbol symbol = semanticModel.GetSymbolInfo(lambda, cancellationToken).Symbol;

            LambdaExpressionSyntax newNode = GetNewNode(lambda, expression, symbol)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(lambda, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static LambdaExpressionSyntax GetNewNode(
            LambdaExpressionSyntax lambda,
            ExpressionSyntax expression,
            ISymbol symbol)
        {
            BlockSyntax block = SyntaxFactory.Block(GetStatement(expression, symbol));

            block = block
                .WithCloseBraceToken(
                    block.CloseBraceToken
                        .WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxHelper.NewLine)));

            switch (lambda.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        return ((SimpleLambdaExpressionSyntax)lambda)
                            .WithBody(block);
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        return ((ParenthesizedLambdaExpressionSyntax)lambda)
                            .WithBody(block);
                    }
            }

            return lambda;
        }

        private static StatementSyntax GetStatement(ExpressionSyntax expression, ISymbol symbol)
        {
            if (symbol?.Kind == SymbolKind.Method)
            {
                var methodSymbol = (IMethodSymbol)symbol;

                if (!methodSymbol.ReturnsVoid)
                    return SyntaxFactory.ReturnStatement(expression);
            }

            return SyntaxFactory.ExpressionStatement(expression);
        }
    }
}
