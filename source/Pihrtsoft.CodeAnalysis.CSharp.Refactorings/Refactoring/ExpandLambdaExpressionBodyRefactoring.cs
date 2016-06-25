// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ExpandLambdaExpressionBodyRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, LambdaExpressionSyntax lambda)
        {
            var expression = lambda.Body as ExpressionSyntax;

            return expression != null
                && expression.Span.Contains(context.Span)
                && context.SupportsSemanticModel;
        }

        public static async Task<Document> RefactorAsync(
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
